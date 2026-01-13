<template>
  <div class="container mt-5" :dir="locale === 'ar' ? 'rtl' : 'ltr'">

    <div class="d-flex justify-content-between align-items-center mb-3">
      <h5 class="mb-0 fw-semibold text-secondary">{{ $t('users.title') }}</h5>
    </div>
    <br />

    <div class="row mb-4 align-items-center" :dir="locale === 'ar' ? 'rtl' : 'ltr'">
      <div class="col-md-8 col-lg-6">
        <div class="input-group custom-search shadow-sm">
          <span v-if="locale === 'en'" class="input-group-text bg-white border-end-0">
            <i class="fas fa-search text-muted"></i>
          </span>

          <input v-model="searchQuery"
                 @input="debouncedSearch"
                 type="text"
                 class="form-control"
                 :class="locale === 'en' ? 'border-start-0' : 'border-end-0'"
                 :placeholder="$t('users.searchPlaceholder')">

          <span v-if="locale === 'ar'" class="input-group-text bg-white border-start-0">
            <i class="fas fa-search text-muted"></i>
          </span>
        </div>
      </div>

      <div class="col-md-4 col-lg-6 mt-3 mt-md-0"
           :class="locale === 'ar' ? 'text-md-start' : 'text-md-end'">
        <button v-if="isAdmin" class="btn btn-add-staff px-4" @click="handleAddStaff">
          <i class="fas fa-user-plus me-2"></i>{{ $t('users.addStaff') }}
        </button>
      </div>
    </div>
    <br />

    <div class="table-card-container shadow-sm">
      <div v-if="loading" class="text-center py-5">
        <div class="spinner-border text-primary" style="width: 4rem; height: 4rem;">
          <span class="visually-hidden">Loading...</span>
        </div>
      </div>

      <div v-else-if="error" class="alert alert-danger mx-3 mt-3">{{ error }}</div>

      <div v-else class="table-responsive">
        <table class="table table-hover custom-table mb-0">
          <thead class="text-center">
            <tr>
              <th @click="sortBy('firstName')" class="cursor-pointer user-select-none">
                {{ $t('users.table.fullName') }} <i :class="sortIcon('firstName')"></i>
              </th>
              <th @click="sortBy('username')" class="cursor-pointer user-select-none">
                {{ $t('users.table.username') }} <i :class="sortIcon('username')"></i>
              </th>
              <th @click="sortBy('email')" class="cursor-pointer user-select-none">
                {{ $t('users.table.email') }} <i :class="sortIcon('email')"></i>
              </th>
              <th @click="sortBy('role')" class="cursor-pointer user-select-none">
                {{ $t('users.table.role') }} <i :class="sortIcon('role')"></i>
              </th>
              <th @click="sortBy('isActive')" class="cursor-pointer user-select-none">
                {{ $t('users.table.status') }} <i :class="sortIcon('isActive')"></i>
              </th>
              <th v-if="isAdmin"></th>
            </tr>
          </thead>
          <tbody class="text-center">
            <tr v-for="user in users" :key="user.id">
              <td>
                <div class="d-flex align-items-center justify-content-start"
                     :class="locale === 'ar' ? 'pe-3' : 'ps-3'">

                  <div class="avatar-sm" :class="locale === 'ar' ? 'ms-3' : 'me-3'">
                    {{ user.firstName[0] }}{{ user.lastName[0] }}
                  </div>

                  <div :class="locale === 'ar' ? 'text-end' : 'text-start'">
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
                          @click="confirmDelete(user.username)"
                          class="btn btn-sm text-danger"
                          title="Delete user">
                    <i class="fas fa-trash"></i>
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>

        <!-- PAGINATION -->
        <div class="pagination-footer mt-3" :dir="locale === 'ar' ? 'rtl' : 'ltr'">
          <div class="text-muted small">
            {{ $t('users.pagination', { count: users.length, total: totalItems }) }}
          </div>

          <div class="d-flex align-items-center gap-2" dir="ltr">
            <button class="page-link" :disabled="currentPage === 1" @click="goToFirst">
              <i class="fa-solid" :class="locale === 'ar' ? 'fa-angles-right' : 'fa-angles-left'"></i>
            </button>
            <button class="page-link" :disabled="currentPage === 1" @click="goToPrev">
              <i class="fa-solid" :class="locale === 'ar' ? 'fa-chevron-right' : 'fa-chevron-left'"></i>
            </button>

            <span class="current-page-display">
              {{ currentPage }} / {{ totalPages || 1 }}
            </span>

            <button class="page-link" :disabled="currentPage === totalPages" @click="goToNext">
              <i class="fa-solid" :class="locale === 'ar' ? 'fa-chevron-left' : 'fa-chevron-right'"></i>
            </button>
            <button class="page-link" :disabled="currentPage === totalPages" @click="goToLast">
              <i class="fa-solid" :class="locale === 'ar' ? 'fa-angles-left' : 'fa-angles-right'"></i>
            </button>
          </div>
        </div>
    </div>
  </div>
    </div>
</template>

<script setup>
  import { ref, onMounted } from 'vue'
  import { useI18n } from 'vue-i18n'
  import Swal from 'sweetalert2'
  import api from '@/components/Authentication Service/AuthAPI'
  import { useAuth } from '@/components/Authentication Service/Authentication'
  import { useConfirmWarning, successDialog, errorDialog, useInputDialog } from '@/components/Modals/Modal'

  const { t, locale } = useI18n()
  const { isAdmin, currentUsername } = useAuth()
  const inputDialog = useInputDialog().inputDialog

  // State
  const users = ref([])
  const currentPage = ref(1)
  const totalPages = ref(1)
  const totalItems = ref(0)
  const perPage = ref(10)
  const loading = ref(true)
  const error = ref('')

  // Search & Sort
  const searchQuery = ref('')
  const sortByField = ref('username')
  const sortDirection = ref('asc')

  // --- Translated Helpers ---

  const getRoleName = (role) => {
    const map = {
      0: t('users.roles.customer'),
      1: t('users.roles.admin'),
      2: t('users.roles.support')
    }
    return map[role] || 'Unknown'
  }

  const getUserStatus = (status) => {
    const map = {
      0: t('users.status.inactive'),
      1: t('users.status.active'),
      2: t('users.status.pending')
    }
    return map[status]
  }

  // --- Logic ---

  const debouncedSearch = debounce(() => {
    currentPage.value = 1
    loadUsers()
  }, 400)

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
      error.value = t('users.errors.failed') + (err.response?.data || err.message)
    } finally {
      loading.value = false
    }
  }

  const handleAddStaff = async () => {
    const isRtl = locale.value === 'ar';
    const html = `
    <div class="text-start" dir="${isRtl ? 'rtl' : 'ltr'}">
      <div class="mb-3">
        <label class="form-label small fw-bold">${t('users.addStaffModal.firstName')}</label>
        <input id="swal-fn" class="form-control" placeholder="${t('users.addStaffModal.placeholders.fn')}">
      </div>
      <div class="mb-3">
        <label class="form-label small fw-bold">${t('users.addStaffModal.lastName')}</label>
        <input id="swal-ln" class="form-control" placeholder="${t('users.addStaffModal.placeholders.ln')}">
      </div>
      <div class="mb-3">
        <label class="form-label small fw-bold">${t('users.addStaffModal.email')}</label>
        <input id="swal-email" type="email" class="form-control" placeholder="${t('users.addStaffModal.placeholders.email')}">
      </div>
      <div class="mb-3">
        <label class="form-label small fw-bold">${t('users.addStaffModal.username')}</label>
        <input id="swal-username" class="form-control" placeholder="${t('users.addStaffModal.placeholders.user')}">
      </div>
    </div>`;

    const preConfirmFn = () => {
      const firstName = document.getElementById('swal-fn').value.trim();
      const lastName = document.getElementById('swal-ln').value.trim();
      const email = document.getElementById('swal-email').value.trim();
      const username = document.getElementById('swal-username').value.trim();

      if (!firstName || !lastName || !email || !username) {
        Swal.showValidationMessage(isRtl ? 'جميع الحقول مطلوبة!' : 'All fields are required!');
        return false;
      }
      return { firstName, lastName, email, username, password: 'temp', confirmPassword: 'temp' };
    };

    const data = await inputDialog(t('users.addStaffModal.title'), html, t('users.addStaffModal.create'), preConfirmFn);

    if (data) {
      try {
        loading.value = true;
        const response = await api.post('/api/Account/AddStaff', data);
        const pwd = response.data.temporaryPassword || response.data;

        await Swal.fire({
          title: t('users.addStaffModal.successTitle') || 'Success!',
          html: `${t('users.addStaffModal.tempPassword')}: <br><b class="fs-4 text-primary">${pwd}</b>`,
          icon: 'success',
          confirmButtonText: t('users.addStaffModal.done') || 'Done'
        });
        loadUsers();
      } catch (err) {
        errorDialog(err.response?.data?.message || err.response?.data || 'Error');
      } finally {
        loading.value = false;
      }
    }
  };

  const confirmDelete = async (username) => {
    const confirmed = await useConfirmWarning().confirmDialog(
      t('users.deleteModal.title'),
      t('users.deleteModal.body', { username }),
      t('users.deleteModal.confirm')
    )
    if (confirmed) {
      await api.delete(`/api/Account`, { params: { username } })
      await successDialog(t('users.success.deleted'))
      loadUsers()
    }
  }

  // Pagination Controls
  const goToPrev = () => { if (currentPage.value > 1) { currentPage.value--; loadUsers(); } }
  const goToNext = () => { if (currentPage.value < totalPages.value) { currentPage.value++; loadUsers(); } }
  const goToFirst = () => { if (currentPage.value !== 1) { currentPage.value = 1; loadUsers(); } }
  const goToLast = () => { if (currentPage.value !== totalPages.value) { currentPage.value = totalPages.value; loadUsers(); } }

  function debounce(fn, delay) {
    let timeout;
    return (...args) => {
      clearTimeout(timeout);
      timeout = setTimeout(() => fn(...args), delay);
    }
  }

  onMounted(loadUsers)
</script>

<style scoped>

  .custom-search {
    border-radius: 8px;
    overflow: hidden;
    border: 1px solid #e2e8f0;
    transition: border-color 0.2s, box-shadow 0.2s;
  }

    .custom-search:focus-within {
      border-color: #46ba86;
      box-shadow: 0 0 0 3px rgba(70, 186, 134, 0.1) !important;
    }

    .custom-search .form-control {
      border: none;
      font-size: 0.95rem;
      padding: 10px 15px;
      box-shadow: none !important; 
    }

    .custom-search .input-group-text {
      border: none;
      padding-left: 15px;
      padding-right: 15px;
    }

  [dir="rtl"] .custom-search .form-control {
    text-align: right;
  }

  .bg-white {
    background-color: #ffffff !important;
  }

  .container {
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    background: #fff;
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
    padding: 12px 9px;
    vertical-align: middle;
  }

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
    flex-shrink: 0; 
  }

  [dir="rtl"] .avatar-sm {
    margin-inline-end: 1rem;
    margin-inline-start: 0;
  }

  .role-badge {
    padding: 4px 12px;
    border-radius: 6px;
    font-size: 0.75rem;
    font-weight: 600;
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

  .status-pill {
    display: inline-flex;
    align-items: center;
    padding: 4px 10px;
    border-radius: 20px;
    font-size: 0.8rem;
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

  .dot {
    width: 6px;
    height: 6px;
    border-radius: 50%;
    margin-inline-end: 6px;
  }

  .active .dot {
    background: #22c55e;
  }

  .inactive .dot {
    background: #ef4444;
  }

  .btn-add-staff {
    background: #46ba86 !important;
    color: white;
    border-radius: 8px;
    font-weight: 600;
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
</style>
