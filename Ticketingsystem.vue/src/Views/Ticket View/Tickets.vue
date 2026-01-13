<template>
  <div class="container mt-5" :dir="locale === 'ar' ? 'rtl' : 'ltr'">
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h5 class="mb-0 fw-semibold text-secondary">{{ $t('tickets.title') }}</h5>
    </div>
    <br />

    <div class="row mb-4 align-items-center">
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
                 :placeholder="$t('tickets.searchPlaceholder')">

          <span v-if="locale === 'ar'" class="input-group-text bg-white border-start-0">
            <i class="fas fa-search text-muted"></i>
          </span>
        </div>
      </div>

      <div class="col-md-4 col-lg-6 mt-3 mt-md-0" :class="locale === 'ar' ? 'text-md-start' : 'text-md-end'">
        <button class="btn btn-add px-4" @click="AddTicketModal">
          <i class="fas fa-plus-circle" :class="locale === 'ar' ? 'ms-2' : 'me-2'"></i>
          {{ $t('tickets.addTicket') }}
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
      <div v-else-if="error" class="alert alert-danger">{{ error }}</div>

      <div v-else class="table-responsive">
        <table class="table table-hover custom-table mb-0">
          <thead class="text-center">
            <tr>
              <th @click="sortBy('title')" class="cursor-pointer user-select-none">
                {{ $t('tickets.table.title') }} <i :class="sortIcon('title')"></i>
              </th>
              <th @click="sortBy('productName')" class="cursor-pointer user-select-none">
                {{ $t('tickets.table.product') }} <i :class="sortIcon('productName')"></i>
              </th>
              <th @click="sortBy('priority')" class="cursor-pointer user-select-none">
                {{ $t('tickets.table.priority') }} <i :class="sortIcon('priority')"></i>
              </th>
              <th @click="sortBy('status')" class="cursor-pointer user-select-none">
                {{ $t('tickets.table.status') }} <i :class="sortIcon('status')"></i>
              </th>
              <th @click="sortBy('createdBy')" class="cursor-pointer user-select-none">
                {{ $t('tickets.table.createdBy') }} <i :class="sortIcon('createdBy')"></i>
              </th>
              <th @click="sortBy('assignedTo')" class="cursor-pointer user-select-none">
                {{ $t('tickets.table.assignedTo') }} <i :class="sortIcon('assignedTo')"></i>
              </th>
              <th @click="sortBy('createDate')" class="cursor-pointer user-select-none">
                {{ $t('tickets.table.date') }} <i :class="sortIcon('createDate')"></i>
              </th>
              <th></th>
            </tr>
          </thead>
          <tbody class="text-center">
            <tr v-for="ticket in tickets" :key="ticket.id">
              <td><strong>{{ ticket.title }}</strong></td>
              <td><strong>{{ ticket.productName }}</strong></td>
              <td>
                <span class="modern-badge" :class="getPriorityClass(ticket.priority)">
                  {{ getPriorityName(ticket.priority) }}
                </span>
              </td>
              <td>
                <span class="modern-badge" :class="getStatusClass(ticket.status)">
                  {{ getStatusName(ticket.status) }}
                </span>
              </td>
              <td>{{ ticket.createdByFullName }}</td>
              <td>
                <div class="d-flex align-items-center justify-content-center gap-2">
                  <div v-if="ticket.assignedTo === null">
                    <select v-if="isAdmin"
                            class="form-select form-select-sm modern-badge-select"
                            :value="ticket.assignedTo"
                            @change="handleAdminAssign(ticket.id, $event.target.value)">
                      <option disabled value="">{{ $t('tickets.status.unassigned') }}</option>
                      <option v-for="staff in staffList" :key="staff.id" :value="staff.id">
                        {{ staff.firstName }} {{ staff.lastName }}
                      </option>
                    </select>

                    <button v-else-if="isStaff"
                            @click="assignToMe(ticket.id)"
                            class="btn btn-sm btn-outline-success border-0 p-1">
                      <i class="fa-solid fa-square-plus" :class="locale === 'ar' ? 'ms-1' : 'me-1'"></i>
                      {{ $t('tickets.status.assignMe') }}
                    </button>
                  </div>

                  <span v-else :class="ticket.assignedTo ? 'text-dark' : 'text-muted italic'">
                    {{ ticket.assignedToFullName || $t('tickets.status.unassigned') }}
                  </span>
                </div>
              </td>
              <td>{{ formatDate(ticket.createDate) }}</td>
              <td>
                <div class="d-flex justify-content-center gap-2">
                  <router-link :to="`/app/ticket/${ticket.id}`" class="btn btn-sm text-primary">
                    <i class="fa-solid fa-eye"></i>
                  </router-link>
                </div>
              </td>
            </tr>
          </tbody>
        </table>

        <div v-if="tickets.length === 0" class="text-center py-5">
          <i class="fas fa-folder-open fa-4x text-muted opacity-50"></i>
          <h4 class="text-muted">{{ $t('tickets.empty') }}</h4>
          <p v-if="searchQuery" class="text-muted">
            {{ $t('tickets.noResults', { query: searchQuery }) }}
          </p>
        </div>
      </div>

      <div class="pagination-footer">
        <div class="text-muted small">
          {{ $t('tickets.pagination', { count: tickets.length, total: totalItems }) }}
        </div>

        <div class="d-flex align-items-center gap-2" :dir="locale === 'ar' ? 'ltr' : 'ltr'">
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
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import Swal from 'sweetalert2';
import { useConfirm, useConfirmWarning, useInputDialog, successDialog, errorDialog } from '@/components/Modals/Modal';
import { useAuth } from '@/components/Authentication Service/Authentication';
import api from '@/components/Authentication Service/AuthAPI';

const { t, locale } = useI18n();
const route = useRoute();
const router = useRouter();

const tickets = ref([]);
const staffList = ref([]);
const currentPage = ref(1);
const totalPages = ref(1);
const totalItems = ref(0);
const perPage = ref(10);
const loading = ref(true);
const error = ref('');

const { isAdmin, isStaff } = useAuth();
const confirmDialog = useConfirm().confirmDialog;
const inputDialog = useInputDialog().inputDialog;

const searchQuery = ref('');
const sortByField = ref('title');
const sortDirection = ref('asc');
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
    });
    const data = response.data;
    tickets.value = data.items || data;
    totalItems.value = data.totalCount || data.length;
    totalPages.value = Math.ceil(totalItems.value / perPage.value);
  } catch (err) {
    error.value = t('tickets.errors.failed') + ': ' + (err.response?.data || err.message);
  } finally {
    loading.value = false;
  }
};

const fetchStaff = async () => {
  try {
    const response = await api.get('/api/Account/Staff');
    staffList.value = response.data;
  } catch (err) { console.error(err); }
};

const getStatusName = (val) => {
  const keys = ['new', 'progress', 'resolved', 'closed', 'reopened'];
  return t(`tickets.status.${keys[val]}`);
};

const getPriorityName = (val) => {
  const keys = ['low', 'medium', 'high'];
  return t(`tickets.priority.${keys[val]}`);
};

  const formatDate = (d) => {
    if (!d) return 'N/A';
    const date = new Date(d);
    const datePart = date.toLocaleDateString('en-GB');
    return datePart;
  };


function debounce(fn, delay) {
  let timeout;
  return (...args) => {
    clearTimeout(timeout);
    timeout = setTimeout(() => fn(...args), delay);
  };
}

const debouncedSearch = debounce(() => {
  currentPage.value = 1;
  fetchTickets();
}, 400);

function sortBy(field) {
  if (sortByField.value === field) {
    sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc';
  } else {
    sortByField.value = field;
    sortDirection.value = 'asc';
  }
  fetchTickets();
}

function sortIcon(field) {
  const marginClass = locale.value === 'ar' ? 'me-2' : 'ms-2';
  if (sortByField.value !== field) return `fas fa-sort ${marginClass} opacity-50`;
  return sortDirection.value === 'asc'
    ? `fas fa-sort-up ${marginClass} text-primary`
    : `fas fa-sort-down ${marginClass} text-primary`;
}

// --- MODALS ---
const AddTicketModal = async () => {
  const productOptions = productList.value.map(p =>
    `<option value="${p.id}">${p.productName}</option>`).join('');

  const html = `
    <div class="mb-3 text-start" dir="${locale.value === 'ar' ? 'rtl' : 'ltr'}">
      <label class="form-label fw-bold">${t('tickets.modal.ticketTitle')}</label>
      <input id="swal-title" type="text" class="form-control" placeholder="${t('tickets.modal.placeholders.title')}">
    </div>
    <div class="mb-3 text-start" dir="${locale.value === 'ar' ? 'rtl' : 'ltr'}">
      <label class="form-label fw-bold">${t('tickets.modal.description')}</label>
      <textarea id="swal-desc" class="form-control" rows="3" placeholder="${t('tickets.modal.placeholders.desc')}"></textarea>
    </div>
    <div class="mb-3 text-start" dir="${locale.value === 'ar' ? 'rtl' : 'ltr'}">
      <label class="form-label fw-bold">${t('tickets.modal.product')}</label>
      <select id="swal-product" class="form-select">
        <option value="">${t('tickets.modal.selectProduct')}</option>
        ${productOptions}
      </select>
    </div>
    <div class="mb-3 text-start" dir="${locale.value === 'ar' ? 'rtl' : 'ltr'}">
      <label class="form-label fw-bold">${t('tickets.modal.attachments')}</label>
      <input id="swal-files" type="file" class="form-control" multiple>
    </div>`;

  const preConfirmFn = () => {
    const title = document.getElementById('swal-title').value.trim();
    const description = document.getElementById('swal-desc').value.trim();
    const productId = document.getElementById('swal-product').value;
    const files = document.getElementById('swal-files').files;

    if (!title || !description || !productId) {
      Swal.showValidationMessage(t('users.errors.failed'));
      return false;
    }
    return { title, description, productId, files };
  };

  const data = await inputDialog(t('tickets.modal.title'), html, t('users.addStaffModal.create'), preConfirmFn);
  if (data) await AddTicket(data);
};

const AddTicket = async (ticketData) => {
  try {
    loading.value = true;
    const formData = new FormData();
    formData.append('Title', ticketData.title);
    formData.append('Description', ticketData.description);
    formData.append('productId', ticketData.productId);
    if (ticketData.files) {
      for (let i = 0; i < ticketData.files.length; i++) {
        formData.append('Files', ticketData.files[i]);
      }
    }
    await api.post(`/api/Ticket`, formData);
    successDialog("Success", t('changePassword.success'));
    fetchTickets();
  } catch (err) {
    errorDialog("Error", err.message);
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

const getPriorityClass = (val) => ({ 0: 'priority-low', 1: 'priority-medium', 2: 'priority-high' }[val] || '');
const getStatusClass = (val) => ({ 0: 'status-new', 1: 'status-progress', 2: 'status-resolved', 3: 'status-closed', 4: 'status-reopened' }[val] || '');

onMounted(() => {
    fetchTickets();
    fetchProductList();
    if (isAdmin.value) {
      fetchStaff(); 
    }

    if (route.query.openModal === 'true') {
      AddTicketModal();
      router.replace({ query: {} });
    }
  });
</script>

<style scoped>
  .container {
    font-family: 'Segoe UI';
    background: #fff;
  }

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

  .btn-add {
    background: #46ba86 !important;
    border: none;
    color: white;
    font-weight: 600;
    border-radius: 8px;
    padding: 10px 24px;
    transition: all 0.3s ease;
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
  }

  .custom-search {
    border-radius: 8px;
    overflow: hidden;
    border: 1px solid #e2e8f0;
  }

    .custom-search .form-control {
      border: none;
      padding: 10px;
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

  .modern-badge {
    padding: 4px 12px;
    border-radius: 6px;
    font-size: 0.85rem;
    font-weight: 600;
    min-width: 100px;
    display: inline-block;
  }

  .priority-low {
    background: #e0f2f1;
    color: #00796b;
  }

  .priority-medium {
    background: #fff3e0;
    color: #ef6c00;
  }

  .priority-high {
    background: #ffebee;
    color: #c62828;
  }

  .status-new {
    background-color: #e3f2fd;
    color: #1565c0;
  }

  .status-progress {
    background-color: #e8eaf6;
    color: #3f51b5;
  }

  .status-resolved {
    background-color: #e8f5e9;
    color: #2e7d32;
  }

  .status-closed {
    background-color: #eceff1;
    color: #455a64;
  }

  .status-reopened {
    background-color: #fff8e1;
    color: #ff8f00;
  }

</style>
