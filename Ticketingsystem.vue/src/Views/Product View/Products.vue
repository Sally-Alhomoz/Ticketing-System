<template>
  <div class="container mt-5">
    <h2 class="mb-4">Products</h2>
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
        <button v-if="isAdmin" class="btn btn-add-product px-4" @click="AddProductModal">
          <i class="fas fa-plus-circle me-2"></i>Add Product
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
              <th @click="sortBy('id')" style="width: 100px; cursor: pointer;">
                Id <i class="fas" :class="sortIcon('id')"></i>
              </th>
              <th @click="sortBy('productName')" style="cursor: pointer;">
                Product Name <i class="fas" :class="sortIcon('productName')"></i>
              </th>
              <th></th>
            </tr>
          </thead>
          <tbody class="text-center">
            <tr v-for="product in products" :key="product.id">
              <td>{{ product.id }}</td>
              <td>{{ product.productName }}</td>
              <td>
                <button @click="confirmDelete(product)" class="btn btn-link text-danger p-0 border-0">
                  <i class="fas fa-trash"></i>
                </button>
              </td>
            </tr>
          </tbody>
        </table>

        <div v-if="products.length === 0" class="text-center py-5">
          <i class="fas fa-box-open fa-3x text-muted mb-3 opacity-50"></i>
          <p class="text-muted">No products found</p>
        </div>
      </div>

      <div class="pagination-footer">
        <div class="text-muted small">
          Showing <strong>{{ products.length }}</strong> of {{ totalItems }}
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
  import { ref, onMounted } from 'vue';
  import axios from 'axios';
  import Swal from 'sweetalert2';
  import Paginate from 'vuejs-paginate-next';
  import api from '@/components/Authentication Service/AuthAPI'
  import { useAuth } from '@/components/Authentication Service/Authentication'
  import { useConfirm, useConfirmWarning, useInputDialog, successDialog, errorDialog } from '@/components/Modals/Modal'

  // State
  const products = ref([]);
  const currentPage = ref(1)
  const totalPages = ref(1)
  const totalItems = ref(0)
  const perPage = ref(5)
  const loading = ref(true)
  const error = ref('')
  const { isAdmin, currentUsername } = useAuth()

  const searchQuery = ref('')
  const sortByField = ref('productName')
  const sortDirection = ref('asc')

  const confirmDialogWarning = useConfirmWarning().confirmDialog
  const confirmDialog = useConfirm().confirmDialog
  const inputDialog = useInputDialog().inputDialog

  // Fetch Data
  const fetchProducts = async () => {
    try {
      const response = await api.get(`/api/Product`, {
        params: {
          page: currentPage.value,
          pageSize: perPage.value,
          search: searchQuery.value.trim(),
          sortBy: sortByField.value,
          sortDirection: sortDirection.value
        }
      });

      const data = response.data
      products.value = data.items || data
      totalItems.value = data.totalCount || data.length
      totalPages.value = Math.ceil(totalItems.value / perPage.value)
    } catch (err) {
      console.error(err);
      errorDialog('Failed to load products');
    } finally {
      loading.value=false
    }
  };

  //Add Product
  const AddProductModal = async () => {
    const html = `
        <div class="mb-3 text-start">
            <label for="swal-title" class="form-label text-dark">Product Name</label>
            <input id="swal-title" type="text" class="form-control" placeholder="Product Name" required>
        </div>`;

    const preConfirmFn = () => {
      const name = document.getElementById('swal-title').value.trim();

      if (!name) {
        Swal.showValidationMessage('Product name is required!');
        return false;
      }
      return { name };
    };

    const data = await inputDialog('Add New Product', html, 'Add', preConfirmFn)
    if (data) {
      const { name } = data;
      await AddProduct(name);
    }

  }

  const AddProduct = async (name) => {
    try {
      await api.post(`/api/Product/Add?name=${encodeURIComponent(name)}`);

      await successDialog(`Product: ${name} Added successfully!`);
    } catch (error) {
      console.error(error);
      const msg = error.response?.data || "Could not add product";
      await errorDialog(msg, "Error");
    }
    await fetchProducts(); 
  }


  //Delete
  const confirmDelete = async (product) => {
    const confirmed = await confirmDialogWarning(
      'Delete Product',
      `Are you sure you want to delete <b>${product.productName}</b>?`,
      'Delete'
    );

    if (confirmed) {
      await deleteProduct(product);
    }
  }

  const deleteProduct= async (product) => {
    try {
      await api.delete(`/api/Product?id=${product.id}`);

      await successDialog(`${product.productName} has been deleted successfully!`)
    } catch (error) {
      let errorMessage = 'An unexpected error occurred during deletion.';

      if (error.response && error.response.data) {
        errorMessage = error.response.data;
      } else if (error.response) {
        errorMessage = `Error ${error.response.status}: Failed to delete the product.`;
      }
      await errorDialog(errorMessage, 'Deletion Failed')
    }
    await fetchProducts();
  }


  const goToPrev = () => {
    if (currentPage.value > 1) {
      currentPage.value--
      fetchProducts()
    }
  }

  const goToNext = () => {
    if (currentPage.value < totalPages.value) {
      currentPage.value++
      fetchProducts()
    }
  }


  const goToFirst = () => {
    if (currentPage.value !== 1) {
      currentPage.value = 1
      fetchProducts()
    }
  }

  const goToLast = () => {
    if (currentPage.value !== totalPages.value) {
      currentPage.value = totalPages.value
      fetchProducts()
    }
  }



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
    fetchProducts()
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
    fetchProducts()
  }

  function sortIcon(field) {
    if (sortByField.value !== field) return 'fas fa-sort ms-2 opacity-50'
    return sortDirection.value === 'asc'
      ? 'fas fa-sort-up ms-2 text-primary'
      : 'fas fa-sort-down ms-2 text-primary'
  }

  const onPageChange = (page) => {
    currentPage.value = page
    fetchProducts()
  }

  onMounted(fetchProducts);
</script>

<style scoped>
  .container {
    font-family: 'Segoe UI', sans-serif;
    background: #ffff;
  }
  h2 {
    font-size: 2.8rem;
    font-weight: 750;
    color: #46ba86;
    text-align: center;
    margin-bottom: 2rem;
  }

  .btn-add-product {
    background: #46ba86 !important;
    border: none;
    color: white;
    font-weight: 600;
    border-radius: 8px;
    padding: 10px 24px;
    transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    box-shadow: 0 4px 6px -1px rgba(6, 78, 59, 0.2);
  }

    .btn-add-product:hover {
      background-color: #065f46;
      color: white;
      transform: translateY(-1px);
      box-shadow: 0 10px 15px -3px rgba(6, 78, 59, 0.3);
    }

    .btn-add-product:active {
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
    font-size:0.9rem;
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

</style>
