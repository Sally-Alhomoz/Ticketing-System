<template>
  <div class="container mt-5">

    <div class="d-flex justify-content-between align-items-center mb-3">
      <h5 class="mb-0 fw-semibold text-secondary">Users</h5>
    </div>
    <br />

    <!-- SEARCH -->
    <div class="row mb-4 align-items-center">
      <div class="col-md-8 col-lg-6">
        <div class="input-group custom-search shadow-sm">
          <span class="input-group-text bg-white border-end-0">
            <i class="fas fa-search text-muted"></i>
          </span>
          <input v-model="searchQuery" @input="debouncedSearch" type="text" class="form-control border-start-0" placeholder="Search users...">
        </div>
      </div>

      <div class="col-md-4 col-lg-6 text-md-end mt-3 mt-md-0">
        <button v-if="isAdmin" class="btn btn-add-staff px-4" @click="handleAddStaff">
          <i class="fas fa-user-plus me-2"></i>Add Staff
        </button>
      </div>
    </div>
    <br />
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
              <th @click="sortBy('firstName')" class="cursor-pointer user-select-none">
                Full Name <i :class="sortIcon('firstName')"></i>
              </th>
              <th @click="sortBy('username')" class="cursor-pointer user-select-none">
                Username <i :class="sortIcon('username')"></i>
              </th>
              <th @click="sortBy('email')" class="cursor-pointer user-select-none">
                Email <i :class="sortIcon('email')"></i>
              </th>
              <th @click="sortBy('role')" class="cursor-pointer user-select-none">
                Role <i :class="sortIcon('role')"></i>
              </th>
              <th @click="sortBy('isActive')" class="cursor-pointer user-select-none">
                Status <i :class="sortIcon('isActive')"></i>
              </th>
              <th v-if="isAdmin"></th>
            </tr>
          </thead>
          <tbody class="text-center">
            <tr v-for="user in users" :key="user.id">
              <td>
                <div class="d-flex align-items-center ps-3">
                  <div class="avatar-sm me-3">
                    {{ user.firstName[0] }}{{ user.lastName[0] }}
                  </div>
                  <div>
                    <div class="fw-bold text-dark">{{ user.firstName }} {{ user.lastName }}</div>
                  </div>
                </div>
              </td>
              <td>{{user.username}}</td>
              <td class="text-muted">{{ user.email }}</td>

              <td>
                <span :class="[ 'role-badge',
                      user.role === 1 ? 'role-admin' :
                      user.role === 2 ? 'role-staff' : 'role-user']">
                  {{ getRoleName(user.role) }}
                </span>
              </td>

              <td>
                <span :class="['status-pill',
                      user.status === 1 ? 'active' :
                      user.status === 2 ? 'pending' : 'inactive']">
                  <span class="dot"></span>
                  {{ getUserStatus(user.status) }}
                </span>
              </td>

              <td v-if="isAdmin">
                <div class="d-flex justify-content-center gap-2">
                  <button v-if="user.username !== currentUsername"
                          @click="confirm(user.username)"
                          class="btn btn-sm text-danger"
                          title="Delete user">
                    <i class="fas fa-trash text-danger"></i>
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>

        <!-- EMPTY STATE -->
        <div v-if="users.length === 0" class="text-center py-5">
          <i class="fas fa-users-slash fa-5x text-muted mb-4 opacity-50"></i>
          <h4 class="text-muted">No user found</h4>
          <p v-if="searchQuery" class="text-muted">
            No results for "<strong>{{ searchQuery }}</strong>"
          </p>
        </div>
      </div>

      <!-- PAGINATION -->
      <div class="pagination-footer">
        <div class="text-muted small">
          Showing <strong>{{ users.length }}</strong> of {{ totalItems }}
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
  import { ref, computed, onMounted } from 'vue'
  import Swal from 'sweetalert2'
  import api from '@/components/Authentication Service/AuthAPI'
  import { useAuth } from '@/components/Authentication Service/Authentication'
  import { useConfirmWarning, successDialog, errorDialog, useInputDialog } from '@/components/Modals/Modal'

  // State
  const users = ref([])
  const currentPage = ref(1)
  const totalPages = ref(1)
  const totalItems = ref(0)
  const perPage = ref(10)
  const loading = ref(true)
  const error = ref('')
  const { isAdmin, currentUsername } = useAuth()
  const inputDialog = useInputDialog().inputDialog

  // Search & Sort
  const searchQuery = ref('')
  const sortByField = ref('username')
  const sortDirection = ref('asc')

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
    loadUsers()
  }

  function sortIcon(field) {
    if (sortByField.value !== field) return 'fas fa-sort ms-2 opacity-50'
    return sortDirection.value === 'asc'
      ? 'fas fa-sort-up ms-2 text-primary'
      : 'fas fa-sort-down ms-2 text-primary'
  }

  const getRoleName = (role) => ({ 0: 'Customer', 1: 'Admin', 2: 'Support' }[role] || 'Unknown')
  const getUserStatus = (status) => ({ 0: 'inActive', 1: 'Active', 2: 'Pending' }[status])

  // Load users
  const loadUsers = async () => {
    loading.value = true
    error.value = ''

    try {
      const response = await api.get(`/api/Account`, {
        params: {
          page: currentPage.value,
          pageSize: perPage.value,
          search: searchQuery.value.trim(),
          sortBy: sortByField.value,
          sortDirection: sortDirection.value
        }
      })

      const data = response.data
      users.value = data.items || data
      totalItems.value = data.totalCount || data.length
      totalPages.value = Math.ceil(totalItems.value / perPage.value)
    } catch (err) {
      error.value = 'Failed to load users: ' + (err.response?.data || err.message)
      console.error(err)
    } finally {
      loading.value = false
    }
  }


  const goToPrev = () => {
    if (currentPage.value > 1) {
      currentPage.value--
      loadUsers();
    }
  }

  const goToNext = () => {
    if (currentPage.value < totalPages.value) {
      currentPage.value++
      loadUsers();
    }
  }


  const goToFirst = () => {
    if (currentPage.value !== 1) {
      currentPage.value = 1
      loadUsers();
    }
  }

  const goToLast = () => {
    if (currentPage.value !== totalPages.value) {
      currentPage.value = totalPages.value
      loadUsers();
    }
  }

  const onPageChange = (page) => {
    currentPage.value = page
    loadUsers()
  }

  const DeleteUser = async (username) => {
    await api.delete(`/api/Account`, {
      params: { username }
    })
    await successDialog('User deleted successfully.')
    loadUsers()
  }

  const handleAddStaff = async () => {
    const html = `
    <div class="text-start">
      <div class="mb-3">
        <label class="form-label small fw-bold">First Name</label>
        <input id="swal-fn" class="form-control" placeholder="Enter first name">
      </div>
      <div class="mb-3">
        <label class="form-label small fw-bold">Last Name</label>
        <input id="swal-ln" class="form-control" placeholder="Enter last name">
      </div>
      <div class="mb-3">
        <label class="form-label small fw-bold">Email</label>
        <input id="swal-email" type="email" class="form-control" placeholder="Enter email address">
      </div>
      <div class="mb-3">
        <label class="form-label small fw-bold">Username</label>
        <input id="swal-username" class="form-control" placeholder="Assign a username">
      </div>
    </div>`;

    const preConfirmFn = () => {
      const firstName = document.getElementById('swal-fn').value.trim();
      const lastName = document.getElementById('swal-ln').value.trim();
      const email = document.getElementById('swal-email').value.trim();
      const username = document.getElementById('swal-username').value.trim();

      if (!firstName || !lastName || !email || !username) {
        Swal.showValidationMessage('All fields are required!');
        return false;
      }
      return { firstName, lastName, email, username, password: 'temp', confirmPassword: 'temp' };
    };

    const data = await inputDialog('Add New Staff Member', html, 'Create', preConfirmFn);

    if (data) {
      try {
        loading.value = true;
        const response = await api.post('/api/Account/AddStaff', data);

        const pwd = response.data.temporaryPassword || response.data;

        await Swal.fire({
          title: 'Staff Created!',
          html: `Temporary Password: <br><b class="fs-4 text-primary">${pwd}</b>`,
          icon: 'success',
          confirmButtonText: 'Done',
          cancelButtonColor:'#d33'
        });

        loadUsers();
      } catch (err) {
        console.error("Staff Creation Error:", err);
        if (err.response?.status !== 200) {
          errorDialog(err.response?.data?.message || err.response?.data || 'Failed to create staff');
        }
        } finally {
        loading.value = false;
      }
    }
  };

  const confirm = async (username) => {
    const confirmed = await useConfirmWarning().confirmDialog(
      'Delete user?',
      `Delete <strong>${username}</strong>? This action cannot be undone.`,
      'Delete'
    )
    if (confirmed) await DeleteUser(username)
  }

  // Debounce helper
  function debounce(fn, delay) {
    let timeout
    return (...args) => {
      clearTimeout(timeout)
      timeout = setTimeout(() => fn(...args), delay)
    }
  }

  onMounted(() => {
    loadUsers()
  })
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

  /* Avatar Style */
  .avatar-sm {
    width: 38px;
    height: 38px;
    background: #eef2ff;
    color: #46ba86;
    font-weight: bold;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 10px;
    font-size: 0.85rem;
    border: 1px solid #e2e8f0;
  }

  /* Role Badges */
  .role-badge {
    padding: 4px 12px;
    border-radius: 6px;
    font-size: 0.75rem;
    font-weight: 600;
    text-transform: uppercase;
  }

  .role-admin {
    background: #fef3c7;
    color: #92400e;
  }

  .role-user {
    background: #f1f5f9;
    color: #475569;
  }
  .role-staff {
    background: #e0e7ff; 
    color: #4338ca; 
  }

  /* Status Pills */
  .status-pill {
    display: inline-flex;
    align-items: center;
    padding: 4px 10px;
    border-radius: 20px;
    font-size: 0.8rem;
    font-weight: 500;
  }

    .status-pill.active {
      background: #dcfce7;
      color: #166534;
    }

    .status-pill.inactive {
      background: #fee2e2;
      color: #991b1b;
    }

    .status-pill.pending {
      background: #fef3c7; 
      color: #92400e;
    }

  .pending .dot {
    background: #f59e0b;
  }

  .dot {
    width: 6px;
    height: 6px;
    border-radius: 50%;
    margin-right: 6px;
  }

  .active .dot {
    background: #22c55e;
    box-shadow: 0 0 0 2px rgba(34, 197, 94, 0.2);
  }

  .inactive .dot {
    background: #ef4444;
  }


  .btn-add-staff {
    background: #46ba86 !important;
    border: none;
    color: white;
    font-weight: 600;
    border-radius: 8px;
    padding: 10px 24px;
    transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    box-shadow: 0 4px 6px -1px rgba(6, 78, 59, 0.2);
  }

    .btn-add-staff:hover {
      background-color: #065f46;
      color: white;
      transform: translateY(-1px);
      box-shadow: 0 10px 15px -3px rgba(6, 78, 59, 0.3);
    }

  .empty-state {
    border: 2px dashed #cbd5e1;
    border-radius: 16px;
    background: #f8fafc;
    margin: 20px;
  }
</style>

