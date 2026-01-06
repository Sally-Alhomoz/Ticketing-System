<template>
  <div class="split-container">
    <div class="login-side">
      <div class="form-wrapper">
        <div class="brand-header mb-4">
          <h2 class="fw-bold">Create Account</h2>
          <p class="text-muted">Join our ticketing system to get started.</p>
        </div>

        <form @submit.prevent="Register">
          <div class="row">
            <div class="col-md-6 mb-3">
              <label class="form-label small fw-bold text-uppercase">First Name</label>
              <input v-model="firstname" type="text" class="form-control custom-input" placeholder="First Name" required />
            </div>
            <div class="col-md-6 mb-3">
              <label class="form-label small fw-bold text-uppercase">Last Name</label>
              <input v-model="lastname" type="text" class="form-control custom-input" placeholder="Last Name" required />
            </div>
          </div>

          <div class="form-group mb-3">
            <label class="form-label small fw-bold text-uppercase">Username</label>
            <input v-model="username" type="text" class="form-control custom-input" placeholder="Choose a username" required />
          </div>

          <div class="form-group mb-3">
            <label class="form-label small fw-bold text-uppercase">Email Address</label>
            <input v-model="email" type="email" class="form-control custom-input" placeholder="Your email" required />
          </div>

          <div class="form-group mb-3">
            <label class="form-label small fw-bold text-uppercase">Password</label>
            <input v-model="password" type="password" class="form-control custom-input" placeholder="Password" required />
          </div>

          <div class="form-group mb-4">
            <label class="form-label small fw-bold text-uppercase">Confirm Password</label>
            <input v-model="confirmPassword" type="password" class="form-control custom-input" placeholder="Confirm your password" required />
          </div>

          <button type="submit" class="btn-register">
            Sign Up
          </button>
        </form>

        <p class="login-link text-center mt-4">
          Already have an account?
          <router-link to="/" class="text-success fw-bold">Login here</router-link>
        </p>

        <div v-if="error" class="error-msg mt-3">{{ error }}</div>
      </div>
    </div>

    <div class="image-side">
      <div class="shape shape-1"></div>
      <div class="shape shape-2"></div>
      <div class="shape shape-3"></div>

      <div class="overlay-content">
        <div class="icon-box mb-4">
          <i class="fas fa-user-plus fa-3x text-white"></i>
        </div>
        <h3 class="display-4 fw-bold text-white custom-title">Begin Your Journey.</h3>
        <p class="lead text-white-50">Experience the most intuitive ticketing platform designed for modern teams.</p>
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
  const error = ref('')

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
  .split-container {
    display: flex;
    min-height: 100vh;
    background-color: #ffffff;
  }

  .login-side {
    flex: 1;
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 40px;
  }

  .form-wrapper {
    width: 100%;
    max-width: 480px; 
  }

  .custom-input {
    padding: 12px;
    border-radius: 8px;
    border: 1.5px solid #edf2f7;
    background-color: #f8fafc;
    transition: all 0.2s ease;
  }

    .custom-input:focus {
      border-color: #46ba86;
      background-color: #fff;
      box-shadow: 0 0 0 3px rgba(70, 186, 134, 0.1);
      outline: none;
    }

  .btn-register {
    width: 100%;
    padding: 14px;
    background: linear-gradient(135deg, #46ba86 0%, #2d8a63 100%);
    color: white;
    border: none;
    border-radius: 8px;
    font-weight: 600;
    font-size: 1.1rem;
    transition: all 0.3s ease;
    cursor: pointer;
  }

    .btn-register:hover {
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(45, 138, 99, 0.3);
    }

  /* Mesh Gradient Side */
  .image-side {
    flex: 1;
    position: relative;
    background-color: #1a4d38;
    background-image: radial-gradient(at 0% 0%, hsla(158,64%,52%,1) 0, transparent 50%), radial-gradient(at 50% 0%, hsla(161,70%,30%,1) 0, transparent 50%), radial-gradient(at 100% 0%, hsla(155,55%,40%,1) 0, transparent 50%);
    overflow: hidden;
    display: none;
    align-items: center;
    justify-content: center;
    padding: 60px;
  }

  @media (min-width: 992px) {
    .image-side {
      display: flex;
    }
  }

  .shape {
    position: absolute;
    filter: blur(80px);
    opacity: 0.5;
    border-radius: 50%;
  }

  .shape-1 {
    width: 400px;
    height: 400px;
    background: #46ba86;
    top: -100px;
    right: -100px;
  }

  .shape-2 {
    width: 300px;
    height: 300px;
    background: #2d8a63;
    bottom: -50px;
    left: -50px;
  }

  .shape-3 {
    width: 250px;
    height: 250px;
    background: #065f46;
    top: 40%;
    left: 20%;
  }

  .overlay-content {
    position: relative;
    z-index: 1;
    max-width: 700px;
  }

  .icon-box {
    background: rgba(255, 255, 255, 0.1);
    width: 80px;
    height: 80px;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 20px;
    backdrop-filter: blur(10px);
    border: 1px solid rgba(255, 255, 255, 0.2);
  }

  .error-msg {
    color: #dc3545;
    background: #fff5f5;
    padding: 12px;
    border-radius: 8px;
    font-size: 0.85rem;
    border: 1px solid #feb2b2;
  }

  .custom-title {
    font-size: clamp(2rem, 4vw, 3.5rem);
    white-space: nowrap;
  }
</style>
