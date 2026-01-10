<template>
  <div class="container mt-5">

    <div class="d-flex justify-content-between align-items-center mb-3">
      <h5 class="mb-0 fw-semibold text-secondary">Products</h5>
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
        <button v-if="isAdmin" class="btn btn-add-product px-4" @click="AddProductModal">
          <i class="fas fa-plus me-2"></i>Add Product
        </button>
      </div>
    </div>
    <br />

    <div class="table-card-container shadow-sm">

      <div v-if="loading" class="text-center py-5">
        <div class="spinner-border text-primary" style="width: 4rem; height: 4rem;"></div>
      </div>

      <div v-else-if="error" class="alert alert-danger text-center py-4">{{ error }}</div>

      <div v-else>
        <div class="row row-cols-1 row-cols-sm-2 row-cols-md-3 row-cols-lg-4 g-3 g-md-4 mb-5">
          <div v-for="product in products" :key="product.id" class="col">
            <div class="product-card h-100">
              <div class="product-image-container">
                <img v-if="product.imageUrl"
                     :src="product.imageUrl"
                     :alt="product.productName"
                     @error="product.imageUrl = null"
                     class="product-image"
                     loading="lazy">
                <div v-else class="image-placeholder">
                  <i class="fas fa-box fa-2x text-white opacity-50"></i>
                  <span class="placeholder-text">No image</span>
                </div>
              </div>

              <div class="card-body d-flex flex-column p-3">
                <h5 class="card-title mb-2 text-truncate-2">
                  {{ product.productName }}
                </h5>

                <div class="mt-auto d-flex justify-content-between align-items-center pt-2">
                  <span class="badge bg-secondary-subtle text-secondary small">
                    ID: {{ product.id }}
                  </span>

                  <button @click="confirmDelete(product)"
                          class="btn btn-sm btn-delete"
                          title="Delete product">
                    <i class="fas fa-trash-alt"></i>
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Empty state -->
        <div v-if="products.length === 0" class="empty-state text-center py-5 my-5">
          <i class="fas fa-box-open fa-5x text-muted mb-4 opacity-40"></i>
          <h4 class="text-muted mb-2">No products found</h4>
          <p class="text-muted">Try adjusting your search or add a new product</p>
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
  const perPage = ref(8)
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
      const items = data.items || data;

      products.value = items.map(product => ({
        ...product,
        imageUrl: `/images/products/${product.id}.png`
      }));
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

  /*CARD GRID */
  .product-card {
    background: white;
    border-radius: 12px;
    border: 1px solid #e2e8f0;
    overflow: hidden;
    transition: all 0.22s ease;
    box-shadow: 0 2px 8px rgba(0,0,0,0.04);
    height: 100%;
    display: flex;
    flex-direction: column;
  }

    .product-card:hover {
      transform: translateY(-6px);
      box-shadow: 0 12px 24px rgba(0,0,0,0.09);
      border-color: #cbd5e1;
    }

  .card-img-top {
    height: 140px;
    background: linear-gradient(135deg, #a7f3d0 0%, #6ee7b7 100%);
    position: relative;
  }

  .bg-gradient-placeholder {
    background: linear-gradient(135deg, #e2e8f0 0%, #cbd5e1 100%);
  }

  .placeholder-content {
    position: absolute;
    inset: 0;
    display: flex;
    align-items: center;
    justify-content: center;
  }

  .card-body {
    padding: 1.25rem;
    flex: 1;
  }

  .card-title {
    font-size: 1.1rem;
    font-weight: 600;
    color: #1e293b;
    line-height: 1.4;
    margin: 0;
    display: -webkit-box;
    -webkit-line-clamp: 2;
    -webkit-box-orient: vertical;
    overflow: hidden;
  }

  .btn-delete {
    color: #ef4444;
    background: rgba(239, 68, 68, 0.08);
    border: none;
    border-radius: 6px;
    padding: 0.35rem 0.75rem;
    transition: all 0.2s;
  }

    .btn-delete:hover {
      background: rgba(239, 68, 68, 0.15);
      color: #dc2626;
      transform: scale(1.08);
    }

  /* Empty state */
  .empty-state {
    border: 2px dashed #cbd5e1;
    border-radius: 16px;
    background: #f8fafc;
    padding: 4rem 2rem;
  }
  .product-image-container {
    position: relative;
    width: 100%;
    height: 160px;
    overflow: hidden;
    background: #f1f5f9;
  }

  .product-image {
    width: 100%;
    height: 100%;
    object-fit: cover;
    object-position: center;
    transition: transform 0.35s ease;
  }

  .product-card:hover .product-image {
    transform: scale(1.06);
  }

  .image-placeholder {
    height: 100%;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    background: linear-gradient(135deg, #e2e8f0 0%, #cbd5e1 100%);
    color: white;
  }

  .placeholder-text {
    margin-top: 8px;
    font-size: 0.8rem;
    opacity: 0.7;
  }

  .text-truncate-2 {
    display: -webkit-box;
    -webkit-line-clamp: 2;
    -webkit-box-orient: vertical;
    overflow: hidden;
  }

  .product-image {
    width: 100%;
    height: 100%;
    object-fit: cover;
    transition: transform 0.35s ease, opacity 0.3s ease;
    animation: fadeIn 0.5s ease-in;
  }

  @keyframes fadeIn {
    from {
      opacity: 0;
    }

    to {
      opacity: 1;
    }
  }

</style>

