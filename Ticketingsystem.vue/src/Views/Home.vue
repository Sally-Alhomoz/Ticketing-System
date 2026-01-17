<template>
  <div v-if="loading" class="text-center mt-5">
    <div class="spinner-border text-success" role="status">
      <span class="visually-hidden">{{ $t('home.loading') }}</span>
    </div>
  </div>

  <div v-else-if="isCustomer" class="container mt-5">
    <div class="hero-section rounded-4 p-5 mb-5 text-white shadow">
      <div class="row align-items-center">
        <div class="col-md-8">
          <h1 class="display-5 fw-bold">{{ $t('home.greeting', { name: currentUsername }) }}</h1>
          <p class="lead">{{ $t('home.customerSubtitle') }}</p>
        </div>
        <div class="col-md-4 text-md-end">
          <button class="btn btn-light btn-lg px-4 fw-bold text-success"
                  @click="$router.push('/app/tickets?openModal=true')">
            <i class="fas fa-plus-circle mx-2"></i>{{ $t('home.createTicket') }}
          </button>
        </div>
      </div>
    </div>

    <div class="row mb-4 text-center">
      <div class="col-md-4">
        <div class="stat-card shadow-sm border-0 card p-4">
          <div class="icon-circle bg-light-blue text-primary mb-3"><i class="fa-solid fa-ticket fa-2x"></i></div>
          <h4 class="text-muted mb-1">{{ $t('home.myTickets') }}</h4>
          <h2 class="fw-bold">{{ myTickets }}</h2>
        </div>
      </div>
      <div class="col-md-4">
        <div class="stat-card shadow-sm border-0 card p-4">
          <div class="icon-circle bg-light-orange text-warning mb-3"><i class="fas fa-clock fa-2x"></i></div>
          <h4 class="text-muted mb-1">{{ $t('home.inProgress') }}</h4>
          <h2 class="fw-bold">{{ inProgress }}</h2>
        </div>
      </div>
      <div class="col-md-4">
        <div class="stat-card shadow-sm border-0 card p-4">
          <div class="icon-circle bg-light-green text-success mb-3"><i class="fa-regular fa-circle-check fa-2x"></i></div>
          <h4 class="text-muted mb-1">{{ $t('home.resolved') }}</h4>
          <h2 class="fw-bold">{{ Solved }}</h2>
        </div>
      </div>
    </div>
  </div>

  <div v-else-if="isStaff || isAdmin" class="container mt-5">
    <div class="hero-section rounded-4 p-5 mb-5 text-white shadow">
      <div class="row align-items-center">
        <div class="col-md-8">
          <h1 class="display-5 fw-bold">{{ $t('home.greeting', { name: currentUsername }) }}</h1>
          <p class="lead">{{ $t('home.staffSubtitle') }}</p>
        </div>
        <div class="col-md-4 text-md-end">
          <router-link to="/app/tickets" class="btn btn-light btn-lg px-4 fw-bold text-success">
            <i class="fas fa-list mx-2"></i>{{ $t('home.browseTickets') }}
          </router-link>
        </div>
      </div>
    </div>

    <div class="row mb-4 text-center">
      <div class="col-md-6 mb-4">
        <div class="stat-card shadow-sm border-0 card p-4">
          <div class="icon-circle bg-light-danger text-danger mb-3"><i class="fas fa-exclamation-triangle fa-2x"></i></div>
          <h4 class="text-muted mb-1">{{ $t('home.unassigned') }}</h4>
          <h2 class="fw-bold">{{ unassignedCount }}</h2>
        </div>
      </div>
      <div class="col-md-6 mb-4">
        <div class="stat-card shadow-sm border-0 card p-4">
          <div class="icon-circle bg-light-blue text-primary mb-3"><i class="fas fa-user-check fa-2x"></i></div>
          <h4 class="text-muted mb-1">{{ $t('home.totalTickets') }}</h4>
          <h2 class="fw-bold">{{ myActiveCount }}</h2>
        </div>
      </div>
      <div class="col-md-6 mb-4">
        <div class="stat-card shadow-sm border-0 card p-4">
          <div class="icon-circle bg-light-green text-success mb-3"><i class="fa-regular fa-circle-check fa-2x"></i></div>
          <h4 class="text-muted mb-1">{{ $t('home.youSolved') }}</h4>
          <h2 class="fw-bold">{{ Solved }}</h2>
        </div>
      </div>
      <div class="col-md-6 mb-4">
        <div class="stat-card shadow-sm border-0 card p-4">
          <div class="icon-circle bg-light-orange text-warning mb-3"><i class="fas fa-clock fa-2x"></i></div>
          <h4 class="text-muted mb-1">{{ $t('home.inProgress') }}</h4>
          <h2 class="fw-bold">{{ myTickets }}</h2>
        </div>
      </div>
    </div>
  </div>

  <div class="container" v-if="history && !loading">
    <div class="row mb-5">
      <div class="col-12">
        <div class="card border-0 shadow-sm p-3 bg-light border-4 activity-card">
          <div class="d-flex align-items-center">
            <div class="flex-grow-1 mx-3">
              <small class="text-uppercase text-muted fw-bold mb-1 d-block" style="font-size: 0.7rem;">
                {{ $t('home.latestActivity') }}
              </small>

              <div class="d-md-flex align-items-center">
                <span class="modern-badge me-2 mb-2 mb-md-0" :class="getStatusClass(history.newStatus)">
                  {{ getStatusName(history.newStatus) }}
                </span>

                <p class="mb-0 text-dark">
                  <span class="text-muted">{{ $t('home.forTicket') }}</span>
                  <strong class="mx-1">"{{ history.ticketTitle }}"</strong>
                  <span class="text-muted mx-2 d-none d-md-inline">|</span>
                  <small class="text-secondary">{{ formatDate(history.changeDate) }}</small>
                </p>
              </div>
            </div>

            <div class="ms-auto">
              <router-link :to="`/app/ticket/${history.ticketId}`"
                           class="btn btn-sm btn-outline-success rounded-pill px-4 fw-bold">
                {{ $t('home.viewDetails') }}
              </router-link>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted } from 'vue'
  import api from '@/components/Authentication Service/AuthAPI'
  import { useI18n } from 'vue-i18n'
  import { useAuth } from '@/components/Authentication Service/Authentication'

  const { locale, t } = useI18n()
  const { isAdmin, isStaff, isCustomer, currentUsername } = useAuth()

  const myTickets = ref(0)
  const Solved = ref(0)
  const inProgress = ref(0)
  const history = ref(null)
  const loading = ref(true)

  const unassignedCount = ref(0)
  const myActiveCount = ref(0)

  const fetchDashboardData = async () => {
    try {
      loading.value = true
      if (isCustomer.value) {
        const res = await api.get('/api/Ticket/DashboardCustomer')
        myTickets.value = res.data.totalTickets || 0
        Solved.value = res.data.solved || 0
        inProgress.value = res.data.inProgress || 0
        history.value = res.data.lastUpdate || null
      }
      else if (isStaff.value || isAdmin.value) {
        const res = await api.get('/api/Ticket/DashboardStaff')
        unassignedCount.value = res.data.unAssigned || 0
        myActiveCount.value = res.data.totalTickets || 0
        Solved.value = res.data.solved || 0
        myTickets.value = res.data.inProgress || 0
      }
    } catch (err) {
      console.error("Dashboard fetch error:", err)
    } finally {
      loading.value = false
    }
  }

  const formatDate = (dateString) => {
    if (!dateString) return ''
    const date = new Date(dateString)
    return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit' })
  }

  const getStatusClass = (val) => {
    const map = {
      0: 'status-new',
      1: 'status-progress',
      2: 'status-resolved',
      3: 'status-closed',
      4: 'status-reopened'
    };
    return map[val] || '';
  };

  const getStatusName = (v) => {
    const statuses = ['new', 'progress', 'resolved', 'closed', 'reopened']
    return t(`tickets.status.${statuses[v]}`)
  }

  onMounted(() => {
    fetchDashboardData()
  })
</script>

<style scoped>
  .hero-section {
    background: linear-gradient(135deg, #46ba86 0%, #2d8a63 100%);
    border: none;
  }

  .stat-card {
    border-radius: 20px;
    background: #ffffff;
    border: 1px solid rgba(0, 0, 0, 0.05) !important;
    transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  }

    .stat-card:hover {
      transform: translateY(-8px);
      box-shadow: 0 15px 30px rgba(0, 0, 0, 0.08) !important;
    }

  .icon-circle {
    width: 60px;
    height: 60px;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 16px;
    margin: 0 auto;
  }

  .activity-card {
    border-radius: 16px;
    border-inline-start-width: 6px !important;
    border-inline-start-style: solid !important;
    border-inline-start-color: #46ba86 !important;
    transition: transform 0.2s ease;
  }

    .activity-card:hover {
      transform: scale(1.005);
    }

  .modern-badge {
    padding: 4px 12px;
    border-radius: 6px;
    font-size: 0.85rem;
    font-weight: 600;
    min-width: 100px;
    display: inline-block;
    text-align: center;
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

  /* --- Helpers --- */
  .bg-light-blue {
    background-color: #eef6ff;
  }

  .bg-light-orange {
    background-color: #fff7ed;
  }

  .bg-light-green {
    background-color: #f0fdf4;
  }

  .bg-light-danger {
    background-color: #fef2f2;
  }

  .btn-light {
    color: #2d8a63 !important;
    border: none;
  }

  [dir="rtl"] {
    font-family: 'Noto Sans Arabic', 'Segoe UI', sans-serif;
  }
</style>
