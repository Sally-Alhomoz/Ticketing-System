<template>
  <div class="container mt-5">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <button class="btn btn-outline-secondary btn-sm shadow-sm" @click="$router.back()">
        <i class="fas fa-arrow-left me-2"></i>Back to List
      </button>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-success" style="width: 3rem; height: 3rem;"></div>
      <p class="mt-2 text-muted">Loading ticket data...</p>
    </div>

    <div v-else-if="ticket" class="row g-4 mb-4">
      <div class="col-lg-8">
        <div class="card shadow-sm border-0 h-100">
          <div class="card-body p-4">
            <h3 class="fw-bold text-dark mb-3">{{ ticket.title }}</h3>

            <div class="description-section bg-light p-4 rounded-3 mb-4">
              <h6 class="text-uppercase small fw-bold mb-2 text-success">Ticket Number</h6>
              <p class="mb-3 fs-6 text-dark">#{{ ticket.id }}</p>

              <h6 class="text-uppercase small fw-bold mb-2 text-success">Description</h6>
              <p class="mb-0 fs-6 text-dark" style="white-space: pre-wrap;">
                {{ ticket.description || 'No description provided.' }}
              </p>
            </div> <div v-if="ticket.attachments?.length" class="mt-4">
              <h6 class="text-uppercase text-muted small fw-bold mb-3">Attachments</h6>
              <div class="d-flex flex-wrap gap-3">
                <div v-for="file in ticket.attachments" :key="file.id" class="attachment-item">
                  <div v-if="isImage(file.fileName)" class="image-wrapper">
                    <img :src="imagePreviews[file.id] || getFileBlobUrl(file.id)"
                         class="img-thumbnail shadow-sm preview-trigger-sm"
                         @click="openPreview(file)" />
                  </div>
                  <div v-else class="file-icon-box" @click="handleFileClick(file)">
                    <i :class="getFileIcon(file.fileName)" class="fa-2x"></i>
                    <small class="d-block mt-1 text-truncate" style="max-width: 80px;">{{ file.fileName }}</small>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

    <div class="col-lg-4">
      <div class="card shadow-sm border-0 h-100">
        <div class="card-header bg-white border-bottom py-3">
          <h6 class="m-0 fw-bold text-dark">Ticket Details</h6>
        </div>
        <div class="card-body p-4">
          <div class="mb-4">
            <label class="text-muted small fw-bold text-uppercase d-block mb-2">Status</label>
            <div class="position-relative">
              <select v-if="currentUserId === ticket.assignedTo || isAdmin"
                      v-model="ticket.status"
                      @change="updateStatus(ticket.id, ticket.status)"
                      :class="['form-select modern-badge-select', getStatusBadgeClass(ticket.status)]">
                <option :value="0">New</option>
                <option :value="1">In Progress</option>
                <option :value="2">Resolved</option>
                <option :value="3">Closed</option>
                <option :value="4">Reopened</option>
              </select>
              <span v-else class="modern-badge d-block text-center" :class="getStatusBadgeClass(ticket.status)">
                {{ getStatusName(ticket.status) }}
              </span>
            </div>
          </div>

          <div class="mb-4">
            <label class="text-muted small fw-bold text-uppercase d-block mb-2">Priority</label>
            <div class="position-relative">
              <select v-if="currentUserId === ticket.assignedTo || isAdmin"
                      v-model="ticket.priority"
                      @change="updatePriority(ticket.id, ticket.priority)"
                      :class="['form-select modern-badge-select text-center', getPriorityBadgeClass(ticket.priority)]">
                <option :value="0">Low</option>
                <option :value="1">Medium</option>
                <option :value="2">High</option>
              </select>
              <div v-else :class="['modern-badge d-block text-center', getPriorityBadgeClass(ticket.priority)]">
                <i class="fas fa-flag me-2"></i> {{ getPriorityName(ticket.priority) }}
              </div>
            </div>
          </div>

        <hr />

        <div class="detail-row mb-3">
          <span class="text-dark">Product:</span>
          <span class="fw-bold float-end text-dark">{{ ticket.productName }}</span>
        </div>
        <div class="detail-row mb-3">
          <span class="text-dark">Agent:</span>
          <span class="fw-bold float-end text-success">{{ ticket.assignedToFullName || 'Unassigned' }}</span>
        </div>
        <div class="detail-row mb-3">
          <span class="text-dark">Created By:</span>
          <span class="fw-bold float-end text-dark">{{ ticket.createdByFullName }}</span>
        </div>
        <div class="detail-row">
          <span class="text-dark">Date:</span>
          <span class="fw-bold float-end text-dark small">{{ formatDate(ticket.createDate) }}</span>
        </div>
      </div>
      </div>
    </div>
  </div>

    <div class="card shadow-sm border-0 mb-5">
      <div class="card-header bg-white p-0 border-bottom">
        <ul class="nav nav-tabs border-0 px-3">
          <li class="nav-item">
            <button class="nav-link py-3 px-4" :class="{active: activeTab === 'comments'}" @click="activeTab = 'comments'">
              <i class="fas fa-comments me-2"></i>Discussion
            </button>
          </li>
          <li class="nav-item" v-if="(isStaff && ticket?.assignedTo === currentUserId) || isAdmin">
            <button class="nav-link py-3 px-4" :class="{active: activeTab === 'history'}" @click="activeTab = 'history'">
              <i class="fas fa-history me-2"></i>Activity History
            </button>
          </li>
        </ul>
      </div>

      <div class="card-body p-4">
        <div v-if="activeTab === 'comments'">
          <div class="mb-5 bg-light p-3 rounded">
            <textarea v-model="newComment" class="form-control border-0 bg-white" rows="3" placeholder="Write a comment..."></textarea>
            <div v-if="selectedFile" class="mt-2 d-flex align-items-center bg-white p-2 rounded border">
              <i class="fas fa-file-alt text-success me-2"></i>
              <span class="small text-truncate">{{ selectedFile.name }}</span>
              <button class="btn btn-sm text-danger ms-auto" @click="clearFile"><i class="fas fa-times"></i></button>
            </div>
            <div class="d-flex justify-content-between align-items-center mt-3">
              <button class="btn btn-light btn-sm text-success" @click="$refs.fileInput.click()">
                <i class="fas fa-paperclip me-2"></i>Attach File
                <input type="file" ref="fileInput" class="d-none" @change="handleFileChange" />
              </button>
              <button class="btn btn-success px-4" @click="postComment" :disabled="(!newComment.trim() && !selectedFile) || posting">
                <span v-if="posting" class="spinner-border spinner-border-sm me-1"></span> Post
              </button>
            </div>
          </div>

          <div v-if="commentsExist">
            <div v-for="comment in ticket.comments" :key="comment.id" class="mb-4 d-flex">
              <div class="flex-shrink-0 me-3">
                <div class="avatar bg-success-subtle text-success fw-bold rounded-circle d-flex align-items-center justify-content-center" style="width: 45px; height: 45px;">
                  {{ comment.createdByFullName.charAt(0) }}
                </div>
              </div>
              <div class="flex-grow-1 border-bottom pb-3">
                <div class="d-flex justify-content-between align-items-center mb-1">
                  <span class="fw-bold text-dark">{{ comment.createdByFullName }}</span>
                  <small class="text-muted">{{ formatDate(comment.createDate) }}</small>
                </div>
                <p class="text-secondary mb-2" style="white-space: pre-wrap;">{{ comment.message }}</p>

                <div v-if="comment.attachments?.length" class="d-flex flex-wrap gap-2 mt-2">
                  <div v-for="file in comment.attachments" :key="file.id" class="d-inline-block text-center">
                    <div v-if="isImage(file.fileName)" class="mb-1">
                      <img :src="imagePreviews[file.id] || getFileBlobUrl(file.id)"
                           class="img-thumbnail shadow-sm preview-trigger-sm"
                           @click="openPreview(file)" />
                    </div>
                    <div class="attachment-chip" @click="handleFileClick(file)">
                      <i :class="getFileIcon(file.fileName)"></i> {{ file.fileName }}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div v-else class="text-center py-5">
            <p class="text-muted">No comments yet. Be the first to start the discussion.</p>
          </div>
        </div>

        <div v-if="activeTab === 'history'">
          <div v-if="historyLoading" class="text-center py-4">
            <div class="spinner-border text-success"></div>
          </div>
          <div v-else>
            <ul class="timeline">
              <li v-for="record in historyRecords" :key="record.id" class="timeline-item">
                <span class="timeline-dot" :class="getStatusBadgeClass(record.newStatus)"></span>
                <div class="timeline-box shadow-sm border p-3 rounded-3">
                  <div class="d-flex justify-content-between mb-1">
                    <span class="fw-bold">{{ record.changedByFullName || 'System' }}</span>
                    <small class="text-muted">{{ formatDate(record.changeDate) }}</small>
                  </div>
                  <p class="mb-0 text-muted">
                    Changed status to <span class="badge" :class="getStatusBadgeClass(record.newStatus)">{{ getStatusName(record.newStatus) }}</span>
                  </p>
                </div>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  </div>
  <div v-if="showModal" class="custom-modal-overlay" @click.self="closeModal">
    <div class="custom-modal-container">
      <div class="custom-modal-header py-2 px-3 d-flex justify-content-between align-items-center bg-dark text-white rounded-top">
        <span class="text-truncate me-3" style="max-width: 250px;">{{ activeFile?.fileName }}</span>
        <div class="d-flex gap-2">
          <button class="btn btn-sm btn-outline-secondary text-white border-secondary" @click="downloadImage(activeFile)">
            <i class="fas fa-download me-1"></i> 
          </button>
          <button class="btn btn-sm btn-danger" @click="closeModal">
            <i class="fas fa-times"></i>
          </button>
        </div>
      </div>
      <div class="bg-black text-center p-2">
        <img :src="imagePreviews[activeFile?.id]" class="custom-modal-img" />
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted, computed, onUnmounted, watch } from 'vue';
  import api from '@/components/Authentication Service/AuthAPI';
  import { useAuth } from '@/components/Authentication Service/Authentication';
  import { successDialog, errorDialog } from '@/components/Modals/Modal';

  const props = defineProps(['id']);
  const ticket = ref(null);
  const loading = ref(true);
  const activeTab = ref('comments');
  const { isAdmin, currentUserId, isStaff } = useAuth();

  const newComment = ref('');
  const posting = ref(false);
  const selectedFile = ref(null);
  const fileInput = ref(null);
  const imagePreviews = ref({});

  const historyRecords = ref([]);
  const historyLoading = ref(false);

  const showModal = ref(false);
  const activeFile = ref(null);

  const commentsExist = computed(() => ticket.value?.comments?.length > 0);

  watch(activeTab, (newVal) => {
    if (newVal === 'history' && historyRecords.value.length === 0) fetchHistory();
  });

  const fetchTicketDetails = async () => {
    try {
      loading.value = true;
      const res = await api.get(`/api/Ticket/GetTicketById?ticketId=${props.id}`);
      ticket.value = res.data;
    } catch (err) { errorDialog('Failed to load ticket.'); }
    finally { loading.value = false; }
  };

  const fetchHistory = async () => {
    try {
      historyLoading.value = true;
      const res = await api.get(`/api/TicketHistory/GetByTicketId?ticketId=${props.id}`);
      historyRecords.value = res.data;
    } catch (err) { console.error("History fetch failed"); }
    finally { historyLoading.value = false; }
  };

  const postComment = async () => {
    try {
      posting.value = true;
      const fd = new FormData();
      fd.append('Message', newComment.value.trim());
      fd.append('TicketId', props.id);
      if (selectedFile.value) fd.append('Files', selectedFile.value);

      await api.post('/api/Comment', fd);
      newComment.value = '';
      selectedFile.value = null;
      await fetchTicketDetails();
      successDialog('Comment posted!');
    } catch (err) { errorDialog('Failed to post.'); }
    finally { posting.value = false; }
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

  const downloadImage = (file) => {
    const url = imagePreviews.value[file.id];
    if (!url || url === 'loading') return;
    const link = document.createElement('a');
    link.href = url;
    link.download = file.fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };

  const getFileBlobUrl = (fileId) => {
    if (imagePreviews.value[fileId]) return imagePreviews.value[fileId];
    imagePreviews.value[fileId] = 'loading';
    api.get(`/api/Attachment/Download/${fileId}`, { responseType: 'blob' })
      .then(res => {
        imagePreviews.value[fileId] = window.URL.createObjectURL(new Blob([res.data]));
      });
    return '';
  };

  const viewPdf = async (fileId) => {
    try {
      const res = await api.get(`/api/Attachment/Download/${fileId}`, { responseType: 'blob' });
      const file = new Blob([res.data], { type: 'application/pdf' });
      const fileURL = URL.createObjectURL(file);
      window.open(fileURL, '_blank');
    } catch (err) {
      errorDialog('Could not open PDF.');
    }
  };

  const isImage = (n) => /\.(jpg|jpeg|png|gif|webp)$/i.test(n);
  const getFileIcon = (n) => n.toLowerCase().endsWith('.pdf') ? 'fas fa-file-pdf text-danger' : 'fas fa-file text-muted';
  const getStatusBadgeClass = (v) => ({ 0: 'status-new', 1: 'status-progress', 2: 'status-resolved', 3: 'status-closed', 4: 'status-reopened' }[v]);
  const getPriorityBadgeClass = (v) => ({ 0: 'priority-low', 1: 'priority-medium', 2: 'priority-high' }[v]);
  const getStatusName = (v) => ['New', 'In Progress', 'Resolved', 'Closed', 'Reopened'][v] || 'Unknown';
  const getPriorityName = (v) => ['Low', 'Medium', 'High'][v] || 'Normal';

  const handleFileChange = (e) => { selectedFile.value = e.target.files[0]; };
  const clearFile = () => { selectedFile.value = null; };
  const getButtonLabel = (f) => isImage(f.fileName) ? 'View' : 'Download';


  const formatDate = (d) => {
    if (!d) return 'N/A';
    const date = new Date(d);

    const datePart = date.toLocaleDateString('en-GB');

    const timePart = date.toLocaleTimeString([], {
      hour: '2-digit',
      minute: '2-digit',
      hour12: true 
    });

    return `${datePart} ${timePart}`;
  };

  const handleFileClick = (file) => {
    const fileName = file.fileName.toLowerCase();

    if (isImage(fileName)) {
      openPreview(file); 
    } else if (fileName.endsWith('.pdf')) {
      viewPdf(file.id);  
    } else {
      downloadFile(file);
    }
  };
  const openPreview = (file) => { activeFile.value = file; showModal.value = true; };
  const closeModal = () => { showModal.value = false; activeFile.value = null; };

  onMounted(fetchTicketDetails);
  onUnmounted(() => {
    Object.values(imagePreviews.value).forEach(url => { if (url !== 'loading') window.URL.revokeObjectURL(url); });
  });
</script>

<style scoped>
  .nav-tabs .nav-link {
    color: #6c757d;
    font-weight: 600;
    border: none;
    border-bottom: 3px solid transparent;
  }

    .nav-tabs .nav-link.active {
      color: #46ba86;
      border-bottom: 3px solid #46ba86;
      background: transparent;
    }

  .timeline {
    list-style: none;
    padding: 0;
    position: relative;
  }

    .timeline::before {
      content: '';
      position: absolute;
      top: 0;
      bottom: 0;
      left: 15px;
      width: 2px;
      background: #e9ecef;
    }

  .timeline-item {
    position: relative;
    padding-left: 50px;
    margin-bottom: 30px;
  }

  .timeline-dot {
    position: absolute;
    left: 9px;
    width: 14px;
    height: 14px;
    border-radius: 50%;
    border: 3px solid white;
    z-index: 2;
  }

  .timeline-box {
    background: #fff;
    transition: 0.3s;
  }

    .timeline-box:hover {
      transform: translateX(5px);
    }

  /* Status */
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
    background-color: #f5f5f5;
    color: #757575;
    border: 1px solid #e0e0e0;
  }

  .status-reopened {
    background-color: #fff8e1;
    color: #ff8f00;
    border: 1px solid #ffecb3;
  }

  .attachment-chip {
    background: #f1f3f5;
    padding: 4px 12px;
    border-radius: 20px;
    font-size: 0.8rem;
    cursor: pointer;
    border: 1px solid #dee2e6;
  }

    .attachment-chip:hover {
      background: #e9ecef;
    }

  /* Thumbnail Previews */
  .preview-trigger-sm {
    height: 60px;
    width: 60px;
    object-fit: cover;
    cursor: pointer;
    border-radius: 4px;
    border: 1px solid #dee2e6;
    transition: transform 0.2s;
  }

    .preview-trigger-sm:hover {
      transform: scale(1.05);
    }

  /* Modal*/
  .custom-modal-overlay {
    position: fixed;
    inset: 0;
    background: rgba(0,0,0,0.9);
    z-index: 9999;
    display: flex;
    justify-content: center;
    align-items: center;
  }

  .custom-modal-container {
    background: #000;
    border-radius: 8px;
    overflow: hidden;
    max-width: 95vw;
    box-shadow: 0 10px 30px rgba(0,0,0,0.5);
  }

  .custom-modal-img {
    max-width: 90vw;
    max-height: 80vh;
    object-fit: contain;
    display: block;
  }

  /* Sidebar Badge Styles */
  .badge-status, .badge-priority {
    padding: 10px;
    border-radius: 6px;
    text-align: center;
    font-weight: 700;
    font-size: 0.85rem;
    letter-spacing: 0.5px;
    text-transform: uppercase;
  }

  /* File Icon Box */
  .file-icon-box {
    width: 80px;
    height: 80px;
    background: #f8f9fa;
    border: 1px border #dee2e6;
    border-radius: 8px;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    cursor: pointer;
    transition: all 0.2s;
  }

    .file-icon-box:hover {
      background: #e9ecef;
      border-color: #adb5bd;
    }


  .detail-row {
    font-size: 0.9rem;
  }

  /*Prioprity levels*/
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

  .modern-badge-select {
    border: 1px solid transparent;
    border-radius: 8px;
    font-weight: 700;
    font-size: 0.85rem;
    text-transform: uppercase;
    padding: 10px;
    cursor: pointer;
    appearance: none;
    text-align: center;
    text-align-last: center;
    background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 16 16'%3e%3cpath fill='none' stroke='black' stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='m2 5 6 6 6-6'/%3e%3c/svg%3e");
    background-repeat: no-repeat;
    background-position: right 0.75rem center;
    background-size: 16px 12px;
  }

    .modern-badge-select:focus {
      box-shadow: 0 0 0 0.25rem rgba(70, 186, 134, 0.2);
      outline: none;
    }

    .modern-badge-select option {
      background-color: #fff;
      color: #333;
      text-transform: none;
      text-align: center;
      font-weight: normal;
    }

  .modern-badge {
    padding: 10px;
    border-radius: 8px;
    text-align: center;
    font-weight: 700;
    font-size: 0.85rem;
    text-transform: uppercase;
    display: block;
  }

  /* Desktop Sidebar Sticky */
  @media (min-width: 992px) {
    .sticky-top {
      position: sticky;
      top: 20px;
    }
  }
</style>
