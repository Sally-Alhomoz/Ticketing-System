<template>
  <div class="container mt-5">
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h5 class="mb-0 fw-semibold text-secondary">Tickets</h5>
    </div>
    <br />

    <div class="row mb-4 align-items-center">
      <div class="col-md-8 col-lg-6">
        <div class="input-group custom-search shadow-sm">
          <span class="input-group-text bg-white border-end-0">
            <i class="fas fa-search text-muted"></i>
          </span>
          <input v-model="searchQuery" @input="debouncedSearch" type="text" class="form-control border-start-0" placeholder="Search products...">
        </div>
      </div>

      <div class="col-md-4 col-lg-6 text-md-end mt-3 mt-md-0">
        <button class="btn btn-add px-4" @click="AddTicketModal">
          <i class="fas fa-plus-circle me-2"></i>Add Ticket
        </button>
      </div>
    </div>
    <br />

    <!-- LOADING / ERROR -->
    <div class="table-card-container shadow-sm">
      <div v-if="loading" class="text-center py-5">
        <div class="spinner-border text-primary" style="width: 4rem; height: 4rem;">
          <span class="visually-hidden">Loading...</span>
        </div>
      </div>
      <div v-else-if="error" class="alert alert-danger">{{ error }}</div>

      <!-- Table -->
      <div v-else class="table-responsive">
        <table class="table table-hover custom-table mb-0">
          <thead class="text-center">
            <tr>
              <th @click="sortBy('title')" class="cursor-pointer user-select-none">
                Title <i :class="sortIcon('title')"></i>
              </th>
              <th @click="sortBy('productName')" class="cursor-pointer user-select-none">
                Product Name <i :class="sortIcon('productName')"></i>
              </th>
              <th @click="sortBy('priority')" class="cursor-pointer user-select-none">
                Priority <i :class="sortIcon('priority')"></i>
              </th>
              <th @click="sortBy('status')" class="cursor-pointer user-select-none">
                Status <i :class="sortIcon('status')"></i>
              </th>
              <th @click="sortBy('createdBy')" class="cursor-pointer user-select-none">
                Created By <i :class="sortIcon('createdBy')"></i>
              </th>
              <th @click="sortBy('assignedTo')" class="cursor-pointer user-select-none">
                Assigned To <i :class="sortIcon('assignedTo')"></i>
              </th>
              <th @click="sortBy('createDate')" class="cursor-pointer user-select-none">
                Create Date <i :class="sortIcon('createDate')"></i>
              </th>
              <th></th>
            </tr>
          </thead>
          <tbody class="text-center">
            <tr v-for="ticket in tickets" :key="ticket.id">
              <td><strong>{{ ticket.title }}</strong></td>
              <td><strong>{{ ticket.productName }}</strong></td>
              <td>
                <div class="d-flex justify-content-center">
                  <select v-if="isStaff"
                          v-model="ticket.priority"
                          @change="updatePriority(ticket.id, ticket.priority)"
                          class="form-select form-select-sm modern-badge-select"
                          :class="getPriorityClass(ticket.priority)">
                    <option :value="0">Low</option>
                    <option :value="1">Medium</option>
                    <option :value="2">High</option>
                  </select>
                  <span v-else class="modern-badge" :class="getPriorityClass(ticket.priority)">
                    {{ getPriorityName(ticket.priority) }}
                  </span>
                </div>
              </td>
              <td>
                <div class="d-flex justify-content-center">
                  <select v-if="isStaff"
                          v-model="ticket.status"
                          @change="updateStatus(ticket.id, ticket.status)"
                          class="form-select form-select-sm modern-badge-select"
                          :class="getStatusClass(ticket.status)">
                    <option :value="0">New</option>
                    <option :value="1">In Progress</option>
                    <option :value="2">Resolved</option>
                    <option :value="3">Closed</option>
                    <option :value="4">Reopened</option>
                  </select>
                  <span v-else class="modern-badge" :class="getStatusClass(ticket.status)">
                    {{ getStatusName(ticket.status) }}
                  </span>
                </div>
              </td>
              <td>{{ ticket.createdByFullName }}</td>
              <td>
                <div class="d-flex align-items-center justify-content-center gap-2">
                  <button v-if="isStaff && ticket.assignedTo===null"
                          @click="assignToMe(ticket.id)"
                          class="btn btn-sm btn-outline-success border-0 p-1"
                          title="Assign to Me">
                    <i class="fa-solid fa-square-plus"></i>
                  </button>
                  <span v-if="ticket.assignedTo" class="text-dark">
                    {{ ticket.assignedToFullName }}
                  </span>
                  <span v-else class="text-muted italic">Unassigned</span>
                </div>
              </td>
              <td>{{formatDate(ticket.createDate)}}</td>
              <td>
                <div class="d-flex justify-content-center gap-2">
                  <router-link :to="`/app/ticket/${ticket.id}`" class="btn btn-sm text-primary" title="View Ticket">
                    <i class="fa-solid fa-eye"></i>
                  </router-link>
                </div>
              </td>
            </tr>
          </tbody>
        </table>

        <!-- EMPTY STATE -->
        <div v-if="tickets.length === 0" class="text-center py-5">
          <i class="fas fa-folder-open fa-4x text-muted opacity-50"></i>
          <h4 class="text-muted">No ticket found</h4>
          <p v-if="searchQuery" class="text-muted">
            No results for "<strong>{{ searchQuery }}</strong>"
          </p>
        </div>
      </div>

      <!-- PAGINATION -->
      <div class="pagination-footer">
        <div class="text-muted small">
          Showing <strong>{{ tickets.length }}</strong> of {{ totalItems }}
        </div>

        <div class="d-flex align-items-center gap-2">

          <!-- FIRST -->
          <button class="page-link"
                  :disabled="currentPage === 1"
                  title="First"
                  @click="goToFirst">
            <i class="fa-solid fa-angles-left"></i>
          </button>

          <!-- PREV -->
          <button class="page-link"
                  :disabled="currentPage === 1"
                  title="Previous"
                  @click="goToPrev">
            <i class="fa-solid fa-chevron-left"></i>
          </button>

          <!-- PAGE INFO -->
          <span class="current-page-display">
            {{ currentPage }} of {{ totalPages || 1 }}
          </span>

          <!-- NEXT -->
          <button class="page-link"
                  :disabled="currentPage === totalPages"
                  title="Next"
                  @click="goToNext">
            <i class="fa-solid fa-chevron-right"></i>
          </button>

          <!-- LAST -->
          <button class="page-link"
                  :disabled="currentPage === totalPages"
                  title="Last"
                  @click="goToLast">
            <i class="fa-solid fa-angles-right"></i>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted, computed, watch } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import Swal from 'sweetalert2';
  import { useConfirm, useConfirmWarning, useInputDialog, successDialog, errorDialog } from '@/components/Modals/Modal'
  import { useAuth } from '@/components/Authentication Service/Authentication'
  import api from '@/components/Authentication Service/AuthAPI'

  const route = useRoute();
  const router = useRouter();

  const tickets = ref([]);

  const currentPage = ref(1)
  const totalPages = ref(1)
  const totalItems = ref(0)
  const perPage = ref(10)
  const loading = ref(true)
  const error = ref('')
  const confirmDialogWarning = useConfirmWarning().confirmDialog
  const confirmDialog = useConfirm().confirmDialog
  const inputDialog = useInputDialog().inputDialog
  const { isAdmin, currentUsername, currentUserId, isStaff } = useAuth()

  const searchQuery = ref('')
  const sortByField = ref('title')
  const sortDirection = ref('asc')
  const selectedStatus = ref('all')
  const productList = ref([]);


const fetchTickets = async () => {
  try {
    const response = await api.get(`/api/Ticket`, {
      params: {
        page: currentPage.value,
        pageSize: perPage.value,
        search: searchQuery.value.trim(),
        sortBy: sortByField.value,
        sortDirection: sortDirection.value
      }
    })

    const data = response.data
    tickets.value = data.items || data
    totalItems.value = data.totalCount || data.length
    totalPages.value = Math.ceil(totalItems.value / perPage.value)
  } catch (error) {
    error.value = 'Failed to load tickets: ' + (err.response?.data || err.message)
    console.error("Failed to load tickets", error);
  }
  finally {
    loading.value = false
  }
};

//Assign ticket to staff
  const assignToMe = async (ticketId) => {
    const confirmed = await confirmDialog(
      "Assign Ticket",
      "Are you sure you want to assign this ticket to yourself?",
      "Assign"
    );
    if (confirmed) {
      await AssignTicket(ticketId)
    }
  };

  const AssignTicket = async (ticketId) => {
    try {
      loading.value = true;

      await api.patch(`/api/Ticket/AssignTo?ticketId=${ticketId}`);

      successDialog("Success", "Ticket assigned to you successfully!");

      await fetchTickets();
    } catch (err) {
      console.error("Assignment error:", err);
      errorDialog("Error", err.response?.data || "Could not assign ticket.");
    } finally {
      loading.value = false;
    }
  };

  //Update ticket priority
  const updatePriority = async (ticketId, newPriority) => {
    try {
      await api.patch(`/api/Ticket/SetPriority?ticketId=${ticketId}&priority=${newPriority}`);
      successDialog("Updated", "Priority changed successfully");
    } catch (err) {
      console.error("Priority update error:", err);
      errorDialog("Error", "Failed to update priority.");
      fetchTickets();
    }
  };

  //Update ticket status
  const updateStatus = async (ticketId, newStatus) => {
    try {
      await api.patch(`/api/Ticket/UpdateStatus?ticketId=${ticketId}&newStatus=${newStatus}`);
      successDialog("Updated", "Status updated successfully");
    } catch (err) {
      console.error("Status update error:", err);
      errorDialog("Error", "Failed to update Status.");
      fetchTickets();
    }
  };

  //Add Ticket
  const AddTicketModal = async () => {

    const productOptions = productList.value.map(p =>
      `<option value="${p.id}">${p.productName}</option>`).join('');

    const html = `
    <div class="mb-3 text-start">
      <label class="form-label fw-bold">Ticket Title</label>
      <input id="swal-title" type="text" class="form-control" placeholder="Enter title">
    </div>
    <div class="mb-3 text-start">
      <label class="form-label fw-bold">Description</label>
      <textarea id="swal-desc" class="form-control" rows="3" placeholder="Describe the issue"></textarea>
    </div>
    <div class="mb-3 text-start">
      <label class="form-label fw-bold">Product</label>
      <select id="swal-product" class="form-select">
        <option value="">-- Select Product --</option>
        ${productOptions}
      </select>
    </div>
    <div class="mb-3 text-start">
      <label class="form-label fw-bold">Attachments</label>
      <input id="swal-files" type="file" class="form-control" multiple>
    </div>`;

    const preConfirmFn = () => {
      const title = document.getElementById('swal-title').value.trim();
      const description = document.getElementById('swal-desc').value.trim();
      const productId = document.getElementById('swal-product').value;
      const files = document.getElementById('swal-files').files;

      if (!title) {
        Swal.showValidationMessage('Please enter a ticket title'); 
        return false;
      }
      if (!description) {
        Swal.showValidationMessage('Please provide a description');
        return false;
      }
      if (!productId) {
        Swal.showValidationMessage('Please select a product');
        return false;
      }

      return {
        title,
        description,
        productId,
        files 
      };
    };

    const data = await inputDialog('Create New Ticket', html, 'Submit', preConfirmFn);
    if (data) {
      await AddTicket(data);
    }
  };

  const AddTicket = async (ticketData) => {
    try {
      loading.value = true;
      const formData = new FormData();

      formData.append('Title', ticketData.title || '');
      formData.append('Description', ticketData.description || '');
      formData.append('productId', ticketData.productId || 0);

      if (ticketData.files && ticketData.files.length > 0) {
        for (let i = 0; i < ticketData.files.length; i++) {
          formData.append('Files', ticketData.files[i]);
        }
      }


      await api.post(`/api/Ticket`, formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      });

      await successDialog("Success", "Ticket submitted successfully!");
      await fetchTickets();
    } catch (err) {
      console.error("Submission error:", err);
      const msg = err.response?.data || "Could not submit ticket";
      await errorDialog("Error", msg);
    } finally {
      loading.value = false;
    }
  };

  const fetchProductList = async () => {
    try {
      const response = await api.get('/api/Product');
      productList.value = response.data.items || response.data;
    } catch (err) {
      console.error("Failed to fetch products for dropdown", err);
    }
  };


// Helpers
  const getStatusName = (val) => ['New', 'InProgress', 'Resolved', 'Closed','Reopened'][val];
  const getPriorityName = (val) => ['Low', 'Medium', 'High'][val];
  const formatDate = (date) => new Date(date).toLocaleDateString();

  // Debounce helper
  function debounce(fn, delay) {
    let timeout
    return (...args) => {
      clearTimeout(timeout)
      timeout = setTimeout(() => fn(...args), delay)
    }
  }

  // Debounce search
  const debouncedSearch = debounce(() => {
    currentPage.value = 1
    fetchTickets()
  }, 400)

  function clearSearch() {
    searchQuery.value = ''
    debouncedSearch()
  }

  // Sorting
  function sortBy(field) {
    if (sortByField.value === field) {
      sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc'
    } else {
      sortByField.value = field
      sortDirection.value = 'asc'
    }
    currentPage.value = 1
    fetchTickets()
  }

  function sortIcon(field) {
    if (sortByField.value !== field) return 'fas fa-sort ms-2 opacity-50'
    return sortDirection.value === 'asc'
      ? 'fas fa-sort-up ms-2 text-primary'
      : 'fas fa-sort-down ms-2 text-primary'
  }

  const getPriorityClass = (val) => {
    const map = { 0: 'priority-low', 1: 'priority-medium', 2: 'priority-high' };
    return map[val] || '';
  };

  const getStatusClass = (val) => {
    const map = {
      0: 'status-new',
      1: 'status-progress',
      2: 'status-resolved',
      3: 'status-closed',
      4: 'status-reopened',
    };
    return map[val] || '';
  };

  const onPageChange = (page) => {
    currentPage.value = page
    fetchTickets()
  }

  onMounted(() => {
    fetchTickets();
    fetchProductList();

    if (route.query.openModal === 'true') {
      AddTicketModal();

      router.replace({ query: {} });
    }
  });
</script>

<style scoped>
  .container {
    font-family: 'Segoe UI';
    background: #ffff;
  }

  h2 {
    font-size: 2.8rem;
    font-weight: 750;
    color: #46ba86;
    text-align: center;
    margin-bottom: 2rem;
  }

  .btn-add {
    background: #46ba86 !important;
    border: none;
    color: white;
    font-weight: 600;
    border-radius: 8px;
    padding: 10px 24px;
    transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    box-shadow: 0 4px 6px -1px rgba(6, 78, 59, 0.2);
  }

    .btn-add:hover {
      background-color: #065f46;
      color: white;
      transform: translateY(-1px);
      box-shadow: 0 10px 15px -3px rgba(6, 78, 59, 0.3);
    }

    .btn-add:active {
      transform: translateY(0);
    }

  .table-card-container {
    border: 1px solid #e2e8f0;
    border-radius: 12px;
    background: #fff;
    overflow: hidden;
  }

  .custom-table thead th {
    background-color: #f8fafc;
    color: #64748b;
    font-size: 0.9rem;
    padding: 14px 16px;
    border-bottom: 1px solid #e2e8f0;
  }

  .custom-table tbody td {
    font-size: 0.9rem;
    padding: 9px;
    vertical-align: middle;
  }

  .custom-table tbody tr:hover {
    background-color: #f8fafc !important;
  }

  .pagination-footer {
    padding: 12px 16px;
    background: #fff;
    border-top: 1px solid #e2e8f0;
    display: flex;
    font-size: 0.9rem;
    justify-content: space-between;
    align-items: center;
  }

  .custom-search {
    border-radius: 8px;
    overflow: hidden;
    border: 1px solid #e2e8f0;
  }

    .custom-search .form-control {
      border: none;
      font-size: 0.95rem;
      padding: 10px;
    }

  .page-link {
    border: none;
    background: transparent;
    color: #46ba86;
    font-size: 0.9rem;
  }

    .page-link:hover:not(:disabled) {
      color: #065f46;
    }

    .page-link:disabled {
      opacity: 0.4;
      cursor: not-allowed;
    }

  .modern-badge, .modern-badge-select {
    padding: 4px 12px;
    border-radius: 6px;
    font-size: 0.85rem;
    font-weight: 600;
    border: none;
    display: inline-block;
    width: auto !important; 
    min-width: 100px;
  }

  .priority-low {
    background-color: #e0f2f1;
    color: #00796b;
    border: 1px solid #b2dfdb;
  }

  .priority-medium {
    background-color: #fff3e0;
    color: #ef6c00;
    border: 1px solid #ffe0b2;
  }

  .priority-high {
    background-color: #ffebee;
    color: #c62828;
    border: 1px solid #ffcdd2;
  }



  .status-new {
    background-color: #e3f2fd;
    color: #1565c0;
    border: 1px solid #bbdefb;
  }

  .status-progress {
    background-color: #e8eaf6;
    color: #3f51b5;
    border: 1px solid #c5cae9;
  }

  .status-resolved {
    background-color: #e8f5e9;
    color: #2e7d32;
    border: 1px solid #c8e6c9;
  }

  .status-closed {
    background-color: #eceff1;
    color: #455a64;
    border: 1px solid #e0e0e0;
  }

  .status-reopened {
    background-color: #fff8e1; 
    color: #ff8f00; 
    border: 1px solid #ffecb3;
  }


  .modern-badge-select {
    cursor: pointer;
    text-align: center;
    appearance: none;
    background-position: right 8px center;
    background-repeat: no-repeat;
    background-size: 8px;
    padding-right: 20px;
  }
</style>
