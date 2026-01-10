<template>
  <div class="auth-viewport">
    <div class="mesh-gradient"></div>
    <div class="glass-sphere sphere-1"></div>
    <div class="glass-sphere sphere-2"></div>

    <div class="container d-flex align-items-center justify-content-center min-vh-100">
      <div class="main-card shadow-2xl">
        <div class="row g-0">

          <div class="col-lg-6 d-none d-lg-flex branding-side">
            <div class="branding-content text-white">
              <div class="glass-pill mb-4">
                <span class="pulse-dot"></span>
                <span class="status-text">System Active 24/7</span>
              </div>
              <h1 class="display-5 fw-black mb-3">Resolve issues <br /> <span class="text-accent">faster than ever.</span></h1>
              <p class="lead opacity-75">The next-generation ticket management system.</p>
            </div>
          </div>

          <div class="col-lg-6 login-side">
            <div class="form-container">
              <div class="text-center mb-5">
                <div class="logo-wrapper mb-3">
                  <i class="fas fa-bolt text-success"></i>
                </div>
                <h3 class="fw-bold text-dark">Welcome Back</h3>
                <p class="text-muted small">Enter your details to access your account.</p>
              </div>

              <form @submit.prevent="handleLogin">
                <div class="input-wrapper mb-3">
                  <label class="input-label">Username</label>
                  <div class="input-field">
                    <i class="far fa-user icon"></i>
                    <input v-model="form.username" type="text" placeholder="username" required />
                  </div>
                </div>

                <div class="input-wrapper mb-4">
                  <label class="input-label">Password</label>
                  <div class="input-field">
                    <i class="fas fa-lock icon"></i>
                    <input v-model="form.password" type="password" placeholder="••••••••" required />
                  </div>
                </div>

                <div v-if="errorMessage" class="alert-modern mb-4">
                  <i class="fas fa-circle-exclamation me-2"></i> {{ errorMessage }}
                </div>

                <button type="submit" class="btn-primary-modern" :disabled="loading">
                  <span v-if="!loading">Sign In <i class="fas fa-arrow-right ms-2"></i></span>
                  <span v-else class="spinner-border spinner-border-sm"></span>
                </button>
              </form>

              <p class="text-center mt-4 mb-0 text-muted small">
                New here? <router-link to="/register" class="text-success fw-bold text-decoration-none">Create an account</router-link>
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, reactive } from 'vue';
  import { useRouter } from 'vue-router';
  import api from '@/components/Authentication Service/AuthAPI';
  import { useAuth } from '@/components/Authentication Service/Authentication';

  const router = useRouter();
  const { login } = useAuth();
  const loading = ref(false);
  const errorMessage = ref('');
  const form = reactive({ username: '', password: '' });

  const handleLogin = async () => {
    loading.value = true;
    errorMessage.value = '';
    try {
      const response = await api.post('/api/Account/Login', {
        username: form.username,
        password: form.password
      });
      login(response.data.token);
      router.push('/app/home');
    } catch (err) {
      errorMessage.value = "Invalid login credentials.";
    } finally {
      loading.value = false;
    }
  };
</script>

<style scoped>
  @import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;600;800&display=swap');

  .auth-viewport {
    background-color: #f8fafc;
    min-height: 100vh;
    font-family: 'Plus Jakarta Sans', sans-serif;
    overflow: hidden;
    position: relative;
  }

  /* 1. ANIMATED BACKGROUND */
  .mesh-gradient {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: radial-gradient(at 0% 0%, rgba(70, 186, 134, 0.15) 0px, transparent 50%), radial-gradient(at 100% 100%, rgba(52, 152, 219, 0.1) 0px, transparent 50%);
    z-index: 0;
  }

  .glass-sphere {
    position: absolute;
    border-radius: 50%;
    filter: blur(60px);
    z-index: 0;
    opacity: 0.4;
  }

  .sphere-1 {
    width: 400px;
    height: 400px;
    background: #46ba86;
    top: -100px;
    right: -50px;
  }

  .sphere-2 {
    width: 300px;
    height: 300px;
    background: #3498db;
    bottom: -50px;
    left: -50px;
  }

  /* 2. MAIN CARD */
  .main-card {
    background: rgba(255, 255, 255, 0.8);
    backdrop-filter: blur(20px);
    border: 1px solid white;
    border-radius: 32px;
    overflow: hidden;
    width: 100%;
    max-width: 1000px;
    z-index: 10;
  }

  /* 3. BRANDING SIDE */
  .branding-side {
    background: linear-gradient(135deg, #1a4d38 0%, #065f46 100%);
    padding: 60px;
    display: flex;
    flex-direction: column;
    justify-content: center;
    position: relative;
  }

  .text-accent {
    color: #46ba86;
  }

  .fw-black {
    font-weight: 800;
    letter-spacing: -1.5px;
  }

  .glass-pill {
    display: inline-flex;
    align-items: center;
    background: rgba(255, 255, 255, 0.1);
    padding: 6px 16px;
    border-radius: 50px;
    font-size: 0.8rem;
    border: 1px solid rgba(255,255,255,0.1);
  }

  .status-dot {
    width: 8px;
    height: 8px;
    background: #46ba86;
    border-radius: 50%;
    margin-right: 10px;
    box-shadow: 0 0 10px #46ba86;
  }

  /* 4. LOGIN SIDE */
  .login-side {
    background: #ffffff;
    padding: 60px;
  }

  .logo-wrapper {
    width: 50px;
    height: 50px;
    background: #f0fdf4;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 12px;
    margin: 0 auto;
    font-size: 1.5rem;
  }

  .input-label {
    font-size: 0.75rem;
    font-weight: 700;
    color: #64748b;
    text-transform: uppercase;
    letter-spacing: 0.5px;
    margin-bottom: 8px;
    display: block;
  }

  .input-field {
    position: relative;
    display: flex;
    align-items: center;
  }

    .input-field i {
      position: absolute;
      left: 15px;
      color: #94a3b8;
    }

    .input-field input {
      width: 100%;
      padding: 12px 12px 12px 45px;
      border-radius: 12px;
      border: 1px solid #e2e8f0;
      background: #f8fafc;
      transition: all 0.2s;
    }

      .input-field input:focus {
        border-color: #46ba86;
        background: #fff;
        outline: none;
        box-shadow: 0 0 0 4px rgba(70, 186, 134, 0.1);
      }

  .btn-primary-modern {
    width: 100%;
    padding: 16px;
    border-radius: 12px;
    background: #1a4d38;
    color: white;
    border: none;
    font-weight: 700;
    transition: 0.3s;
    margin-top: 10px;
  }

    .btn-primary-modern:hover {
      background: #46ba86;
      transform: translateY(-2px);
    }

  .alert-modern {
    background: #fff5f5;
    color: #c53030;
    padding: 12px;
    border-radius: 10px;
    border: 1px solid #feb2b2;
    font-size: 0.85rem;
  }

  .glass-pill {
    display: inline-flex;
    align-items: center;
    background: rgba(255, 255, 255, 0.1);
    padding: 8px 16px;
    border-radius: 50px;
    border: 1px solid rgba(255,255,255,0.1);
  }

  .pulse-dot {
    width: 8px;
    height: 8px;
    background: #46ba86;
    border-radius: 50%;
    margin-right: 12px;
    position: relative;
  }

    .pulse-dot::after {
      content: '';
      position: absolute;
      inset: 0;
      border-radius: 50%;
      background: #46ba86;
      animation: pulse 2s infinite;
    }
  @keyframes pulse {
    0% {
      transform: scale(1);
      opacity: 0.8;
    }

    100% {
      transform: scale(3);
      opacity: 0;
    }
  }
</style>


