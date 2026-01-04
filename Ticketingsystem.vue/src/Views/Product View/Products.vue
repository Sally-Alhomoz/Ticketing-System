<template>
  <div class="container mt-5">
    <h2 class="mb-4">Products Management</h2>
    <br />

    <div class="row mb-4 align-items-center">
      <!-- Search  -->
      <div class="col-lg-6 col-md-7">
        <div class="input-group shadow-sm">
          <span class="input-group-text bg-light border-end-0">
            <i class="fas fa-search text-muted"></i>
          </span>
          <input v-model="searchQuery"
                 @input="debouncedSearch"
                 type="text"
                 class="form-control border-start-0"
                 placeholder="Search..."
                 style="font-size: 0.95rem;" />
          <button v-if="searchQuery"
                  @click="clearSearch"
                  class="btn btn-outline-secondary border-start-0"
                  type="button">
            <i class="fas fa-times"></i>
          </button>
        </div>
      </div>

      <div class="col-lg-6 col-md-5 text-end">
        <button v-if="isAdmin" class="btn btn-primary px-4" @click="AddProductModal">
          <i class="fas fa-plus-circle me-2"></i>Add Product
        </button>
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
              <th @click="sortBy('id')">
                Id
                <i class="fas" :class="sortIcon('id')"></i>
              </th>
              <th @click="sortBy('productName')">
                Product Name
                <i class="fas" :class="sortIcon('productName')"></i>
              </th>
              <th></th>
            </tr>
          </thead>
          <tbody class="text-center">
            <tr v-for="product in products" :key="product.id">
              <td>{{product.id}}</td>
              <td>{{ product.productName }}</td>
              <td>
                <button @click="confirmDelete(product)" class="btn btn-outline-danger btn-sm">
                  <i class="fas fa-trash"></i>
                </button>
              </td>
            </tr>
          </tbody>
        </table>

        <!-- EMPTY STATE -->
        <div v-if="products.length === 0" class="text-center py-5">
          <i class="fas fa-box-open fa-5x text-muted mb-4 opacity-50"></i>
          <h4 class="text-muted">No product found</h4>
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
  import { ref, onMounted } from 'vue';
  import axios from 'axios';
  import api from '@/components/Authentication Service/AuthAPI'
  import { useAuth } from '@/components/Authentication Service/Authentication'
  import { useConfirm, useConfirmWarning, useInputDialog, successDialog, errorDialog } from '@/components/Modals/Modal'

  // State
  const products = ref([]);
  const currentPage = ref(1)
  const totalPages = ref(1)
  const totalItems = ref(0)
  const perPage = ref(10)
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
