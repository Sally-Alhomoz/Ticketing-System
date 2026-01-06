<template>
  <div v-if="loading" class="text-center mt-5">
    <div class="spinner-border text-success" role="status">
      <span class="visually-hidden">Loading...</span>
    </div>
  </div>

  <div v-else-if="isCustomer" class="container mt-5">
    <div class="hero-section rounded-4 p-5 mb-5 text-white shadow">
      <div class="row align-items-center">
        <div class="col-md-8">
          <h1 class="display-5 fw-bold">Hello, {{ currentUsername }}!</h1>
          <p class="lead">Need help with a product? We're here to help you.</p>
        </div>
        <div class="col-md-4 text-md-end">
          <button class="btn btn-light btn-lg px-4 fw-bold text-success"
                  @click="$router.push('/app/tickets?openModal=true')">
            <i class="fas fa-plus-circle me-2"></i>Create New Ticket
          </button>
        </div>
      </div>
    </div>

    <div class="row mb-4 text-center">
      <div class="col-md-4">
        <div class="stat-card shadow-sm border-0 card p-4">
          <div class="icon-circle bg-light-blue text-primary mb-3"><i class="fa-solid fa-ticket fa-2x"></i></div>
          <h4 class="text-muted mb-1">My Tickets</h4>
          <h2 class="fw-bold">{{ myTickets }}</h2>
        </div>
      </div>
      <div class="col-md-4">
        <div class="stat-card shadow-sm border-0 card p-4">
          <div class="icon-circle bg-light-orange text-warning mb-3"><i class="fas fa-clock fa-2x"></i></div>
          <h4 class="text-muted mb-1">In Progress</h4>
          <h2 class="fw-bold">{{ inProgress }}</h2>
        </div>
      </div>
      <div class="col-md-4">
        <div class="stat-card shadow-sm border-0 card p-4">
          <div class="icon-circle bg-light-green text-success mb-3"><i class="fa-regular fa-circle-check fa-2x"></i></div>
          <h4 class="text-muted mb-1">Resolved</h4>
          <h2 class="fw-bold">{{ Solved }}</h2>
        </div>
      </div>
    </div>
  </div>

  <div v-else-if="isStaff || isAdmin" class="container mt-5">
    <div class="hero-section rounded-4 p-5 mb-5 text-white shadow">
      <div class="row align-items-center">
        <div class="col-md-8">
          <h1 class="display-5 fw-bold">Hello, {{ currentUsername }}!</h1>
          <p class="lead">Review and manage customer support tickets.</p>
        </div>
        <div class="col-md-4 text-md-end">
          <router-link to="/app/tickets" class="btn btn-light btn-lg px-4 fw-bold text-success">
            <i class="fas fa-list me-2"></i>Browse All Tickets
          </router-link>
        </div>
      </div>
    </div>

    <div class="row mb-4 text-center">
      <div class="col-md-6 mb-4">
        <div class="stat-card shadow-sm border-0 card p-4">
          <div class="icon-circle bg-light-danger text-danger mb-3"><i class="fas fa-exclamation-triangle fa-2x"></i></div>
          <h4 class="text-muted mb-1">Unassigned Tickets</h4>
          <h2 class="fw-bold">{{ unassignedCount }}</h2>
        </div>
      </div>
      <div class="col-md-6 mb-4">
        <div class="stat-card shadow-sm border-0 card p-4">
          <div class="icon-circle bg-light-blue text-primary mb-3"><i class="fas fa-user-check fa-2x"></i></div>
          <h4 class="text-muted mb-1">Total Tickets</h4>
          <h2 class="fw-bold">{{ myActiveCount }}</h2>
        </div>
      </div>
      <div class="col-md-6 mb-4">
        <div class="stat-card shadow-sm border-0 card p-4">
          <div class="icon-circle bg-light-green text-success mb-3"><i class="fa-regular fa-circle-check fa-2x"></i></div>
          <h4 class="text-muted mb-1">You have Solved</h4>
          <h2 class="fw-bold">{{ Solved }}</h2>
        </div>
      </div>
      <div class="col-md-6 mb-4">
        <div class="stat-card shadow-sm border-0 card p-4">
          <div class="icon-circle bg-light-orange text-warning mb-3"><i class="fas fa-clock fa-2x"></i></div>
          <h4 class="text-muted mb-1">In Progress</h4>
          <h2 class="fw-bold">{{ myTickets }}</h2>
        </div>
      </div>
    </div>
  </div>

  <div class="container" v-if="history && !loading">
    <div class="row mb-5">
      <div class="col-12">
        <div class="card border-0 shadow-sm p-3 bg-light border-start border-success border-4 activity-card">
          <div class="d-flex align-items-center">
            <div class="flex-shrink-0 me-3">
              <div class="icon-circle-sm bg-white shadow-sm text-success text-center">
                <i class="fas fa-bell"></i>
              </div>
            </div>
            <div class="flex-grow-1">
              <small class="text-uppercase text-muted fw-bold" style="font-size: 0.7rem;">Latest Activity</small>
              <p class="mb-0 text-dark">
                <strong>{{ getStatusName(history.newStatus) }}</strong>
                <span class="text-muted"> for ticket: </span>
                <strong>{{ history.ticketTitle }}</strong>
                <span class="text-muted mx-2">|</span>
                <small class="text-secondary">{{ formatDate(history.changeDate) }}</small>
              </p>
            </div>
            <div class="ms-auto">
              <router-link :to="`/app/ticket/${history.ticketId}`" class="btn btn-sm btn-outline-success rounded-pill px-3">
                View Details
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
  import { useAuth } from '@/components/Authentication Service/Authentication'

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

  const getStatusName = (v) => ['New', 'In Progress', 'Resolved', 'Closed', 'Reopened'][v]

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

  .icon-circle-sm {
    width: 40px;
    height: 40px;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 12px;
  }

  .activity-card {
    transition: transform 0.2s ease;
  }

    .activity-card:hover {
      transform: scale(1.01);
    }

  .bg-light-blue {
    background-color: #f0f7ff;
  }

  .bg-light-orange {
    background-color: #fffaf0;
  }

  .bg-light-green {
    background-color: #f0fff4;
  }

  .btn-light {
    color: #2d8a63 !important;
    border: none;
  }

  .bg-light-danger {
    background-color: #fce3e3;
  }

  .bg-light-info {
    background-color: #f0feff;
  }
</style>

