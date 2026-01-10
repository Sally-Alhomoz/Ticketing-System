<template>
  <div class="auth-viewport">
    <div class="mesh-gradient"></div>
    <div class="glass-sphere sphere-1"></div>
    <div class="glass-sphere sphere-2"></div>

    <div class="container d-flex align-items-center justify-content-center min-vh-100">
      <div class="main-card shadow-2xl">
        <div class="row g-0">

          <div class="col-lg-6 d-none d-lg-flex branding-side">
            <div class="branding-content text-white w-100">
              <div class="glass-pill mb-4">
                <span class="pulse-dot"></span>
                <span class="status-text">System Active 24/7</span>
              </div>

              <h1 class="display-5 fw-black mb-3">
                Begin your <br />
                <span class="text-accent">journey with us.</span>
              </h1>
              <p class="lead opacity-75 mb-5">
                Join for high-performance support teams.
              </p>

            </div>
          </div>

          <div class="col-lg-6 login-side">
            <div class="form-container">
              <div class="text-center mb-4">
                <div class="logo-wrapper mb-3">
                  <i class="fas fa-user-plus text-success"></i>
                </div>
                <h3 class="fw-bold text-dark mb-1">Create Account</h3>
                <p class="text-muted small">Sign up to start managing your tickets.</p>
              </div>

              <form @submit.prevent="Register">
                <div class="row g-3 mb-3">
                  <div class="col-md-6">
                    <label class="input-label">First Name</label>
                    <div class="input-field">
                      <i class="far fa-address-card icon"></i>
                      <input v-model="firstname" type="text" placeholder="first name" required />
                    </div>
                  </div>
                  <div class="col-md-6">
                    <label class="input-label">Last Name</label>
                    <div class="input-field">
                      <i class="far fa-address-card icon"></i>
                      <input v-model="lastname" type="text" placeholder="last name" required />
                    </div>
                  </div>
                </div>

                <div class="input-wrapper mb-3">
                  <label class="input-label">Username</label>
                  <div class="input-field">
                    <i class="far fa-user icon"></i>
                    <input v-model="username" type="text" placeholder="Coose a username" required />
                  </div>
                </div>

                <div class="input-wrapper mb-3">
                  <label class="input-label">Email</label>
                  <div class="input-field">
                    <i class="far fa-envelope icon"></i>
                    <input v-model="email" type="email" placeholder="Enter your email" required />
                  </div>
                </div>

                <div class="row g-3 mb-4">
                  <div class="col-md-6">
                    <label class="input-label">Password</label>
                    <div class="input-field">
                      <i class="fas fa-lock icon"></i>
                      <input v-model="password" type="password" placeholder="••••••••" required />
                    </div>
                  </div>
                  <div class="col-md-6">
                    <label class="input-label">Confirm</label>
                    <div class="input-field">
                      <i class="fas fa-shield-alt icon"></i>
                      <input v-model="confirmPassword" type="password" placeholder="••••••••" required />
                    </div>
                  </div>
                </div>

                <button type="submit" class="btn-primary-modern">
                  <span>Create Account <i class="fas fa-arrow-right ms-2 small"></i></span>
                </button>
              </form>

              <p class="text-center mt-4 mb-0 text-muted small">
                Already have an account?
                <router-link to="/" class="text-success fw-bold text-decoration-none">Login here</router-link>
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref } from 'vue'
  import { useRouter } from 'vue-router'
  import api from '@/components/Authentication Service/AuthAPI';
  import { errorDialog } from '@/components/Modals/Modal'

  const router = useRouter()
  const username = ref('')
  const firstname = ref('')
  const lastname = ref('')
  const password = ref('')
  const confirmPassword = ref('')
  const email = ref('')

  const Register = async () => {
    if (password.value !== confirmPassword.value) {
      await errorDialog('Passwords do not match')
      return
    }

    try {
      await api.post('/api/Account/Register', {
        Username: username.value,
        Password: password.value,
        ConfirmPassword: confirmPassword.value,
        FirstName: firstname.value,
        LastName: lastname.value,
        Email: email.value
      })
      router.push('/')
    } catch (err) {
      let errorMessage = 'An unexpected error occurred.';
      if (err.response && err.response.data) {
        errorMessage = err.response.data;
      }
      await errorDialog(errorMessage)
    }
  }
</script>

<style scoped>
  @import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;600;700;800&display=swap');

  .auth-viewport {
    background-color: #f8fafc;
    min-height: 100vh;
    font-family: 'Plus Jakarta Sans', sans-serif;
    overflow: hidden;
    position: relative;
  }

  .mesh-gradient {
    position: absolute;
    inset: 0;
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

  .main-card {
    background: rgba(255, 255, 255, 0.85);
    backdrop-filter: blur(25px);
    border: 1px solid white;
    border-radius: 32px;
    overflow: hidden;
    width: 100%;
    max-width: 1100px;
    z-index: 10;
  }

  .branding-side {
    background: linear-gradient(135deg, #1a4d38 0%, #064e3b 100%);
    padding: 60px;
    display: flex;
    flex-direction: column;
    justify-content: center;
    min-height: 100%;
  }

  .text-accent {
    color: #46ba86;
  }

  .fw-black {
    font-weight: 800;
    letter-spacing: -1.5px;
  }

  /* 24/7 Animation */
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

  /* Stats Row */
  .stat-value {
    font-size: 1.4rem;
    font-weight: 800;
    color: #fff;
  }

  .stat-label {
    font-size: 0.7rem;
    color: rgba(255,255,255,0.6);
    text-transform: uppercase;
  }

  .stat-divider {
    width: 1px;
    height: 35px;
    background: rgba(255,255,255,0.2);
  }
  /* Form Side */
  .login-side {
    background: #ffffff;
    padding: 40px 60px;
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
    font-size: 0.7rem;
    font-weight: 700;
    color: #64748b;
    text-transform: uppercase;
    margin-bottom: 6px;
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
      padding: 10px 10px 10px 42px;
      border-radius: 10px;
      border: 1.5px solid #e2e8f0;
      background: #f8fafc;
      transition: all 0.2s;
      font-size: 0.9rem;
    }

      .input-field input:focus {
        border-color: #46ba86;
        background: #fff;
        outline: none;
        box-shadow: 0 0 0 4px rgba(70, 186, 134, 0.1);
      }

  .btn-primary-modern {
    width: 100%;
    padding: 14px;
    border-radius: 12px;
    background: #1a4d38;
    color: white;
    border: none;
    font-weight: 700;
    transition: 0.3s;
  }

    .btn-primary-modern:hover {
      background: #46ba86;
      transform: translateY(-2px);
    }

  .preview-card {
    background: rgba(255,255,255,0.05);
    padding: 15px;
    border-radius: 12px;
  }

  .preview-line {
    height: 6px;
    background: rgba(255,255,255,0.1);
    border-radius: 3px;
  }
</style>

