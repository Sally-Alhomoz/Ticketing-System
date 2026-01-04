<template>
  <div class="container mt-5">
    <h2>Tickets</h2>

    <!-- SEARCH -->
    <div class="row mb-4">
      <div class="col-lg-5 col-md-6 col-sm-8">
        <div class="input-group shadow-sm">
          <span class="input-group-text bg-light border-end-0">
            <i class="fas fa-search text-muted"></i>
          </span>
          <input v-model="searchQuery"
                 @input="debouncedSearch"
                 type="text"
                 class="form-control border-start-0"
                 placeholder="Search..."
                 aria-label="Search users"
                 style="font-size: 0.95rem;" />
          <button v-if="searchQuery"
                  @click="clearSearch"
                  class="btn btn-outline-secondary border-start-0"
                  type="button">
            <i class="fa-solid fa-magnifying-glass"></i>
          </button>
        </div>
      </div>
    </div>

    <!-- LOADING / ERROR -->
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" style="width: 4rem; height: 4rem;">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    <div v-else-if="error" class="alert alert-danger">{{ error }}</div>

    <!-- Table -->
    <div v-else class="table-responsive">
      <table class="table table-striped table-bordered table-hover table-sm">
        <thead class="text-center bg-dark text-white">
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
          </tr>
        </thead>
        <tbody class="text-center">
          <tr v-for="ticket in tickets" :key="ticket.id">
            <td><strong>{{ ticket.title }}</strong></td>
            <td><strong>{{ ticket.productName }}</strong></td>
            <td>{{ getPriorityName(ticket.priority)}}</td>
            <td>{{  getStatusName(ticket.status) }}</td>
            <td>{{ ticket.createdByFullName }}</td>
            <td>{{ticket.assignedToFullName}}</td>
            <td>{{formatDate(ticket.createDate)}}</td>
          </tr>
        </tbody>
      </table>

      <!-- EMPTY STATE -->
      <div v-if="tickets.length === 0" class="text-center py-5">
        <i class="fas fa-users-slash fa-5x text-muted mb-4 opacity-50"></i>
        <h4 class="text-muted">No ticket found</h4>
        <p v-if="searchQuery" class="text-muted">
          No results for "<strong>{{ searchQuery }}</strong>"
        </p>
      </div>
    </div>

    <!-- PAGINATION -->
    <div class="d-flex justify-content-between align-items-center mt-5 flex-wrap gap-3">
      <paginate v-if="totalPages > 1"
                v-model="currentPage"
                :page-count="totalPages"
                :page-range="5"
                :margin-pages="2"
                :click-handler="onPageChange"
                :prev-text="'Prev'"
                :next-text="'Next'"
                :container-class="'pagination pagination-lg'"
                :page-class="'page-item'"
                :page-link-class="'page-link'"
                :prev-class="'page-item'"
                :next-class="'page-item'"
                :active-class="'active'" />

      <div class="text-muted">
        Page {{ currentPage }} of {{ totalPages || 1 }}
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted, computed } from 'vue';
  import Paginate from 'vuejs-paginate-next'
  import { useConfirm, useConfirmWarning, useInputDialog, successDialog, errorDialog } from '@/components/Modals/Modal'
  import { useAuth } from '@/components/Authentication Service/Authentication'
  import api from '@/components/Authentication Service/AuthAPI'

  const tickets = ref([]);

  const currentPage = ref(1)
  const totalPages = ref(1)
  const totalItems = ref(0)
  const perPage = ref(10)
  const loading = ref(true)
  const error = ref('')
  const confirmDialogWarning = useConfirmWarning().confirmDialog
  const confirmDialog = useConfirm().confirmDialog
  const { isAdmin, currentUsername } = useAuth()

  const searchQuery = ref('')
  const sortByField = ref('title')
  const sortDirection = ref('asc')


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


// Helpers
  const getStatusName = (val) => ['New', 'InProgress', 'Resolved', 'Closed','Reopened','Deleted'][val];
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
    loadUsers()
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


  onMounted(fetchTickets);

</script>

<style scoped>
  .container-fluid {
    padding-left: 0;
    padding-right: 0;
  }
  h2 {
    font-size: 2.8rem;
    font-weight: 750;
    color: #46ba86;
    text-align: center;
    margin-bottom: 2rem;
  }

  /* Search Bar */
  .input-group {
    max-width: 500px;
    margin: 0 auto 1rem;
    display: flex;
    background: white;
    border-radius: 20px;
    overflow: hidden;
    box-shadow: 0 10px 30px rgba(70, 186, 134, 0.15);
    border: 1px solid rgba(70, 186, 134, 0.2);
  }

  .input-group-text {
    background: transparent !important;
    border: none;
    padding: 0 1.2rem;
  }

  .form-control {
    border: none !important;
    padding: 1.1rem 1rem;
    font-size: 1rem;
    box-shadow: none !important;
  }

    .form-control:focus {
      box-shadow: none !important;
    }

  .input-group button {
    border: none;
    background: transparent;
    padding: 0 1.2rem;
    color: #94a3b8;
  }
</style>
