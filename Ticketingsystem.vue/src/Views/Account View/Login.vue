<template>
  <div class="split-container">
    <div class="login-side">
      <div class="form-wrapper">
        <div class="brand-header mb-4">
          <h2 class="fw-bold">Welcome Back</h2>
          <p class="text-muted">Please enter your details to login.</p>
        </div>

        <form @submit.prevent="handleLogin">
          <div class="form-group mb-3">
            <i class="fa-regular fa-user text-muted me-2"></i>
            <label class="form-label small fw-bold text-uppercase">Username</label>
            <input v-model="form.username"
                   type="text"
                   class="form-control custom-input"
                   placeholder="Enter username"
                   required />
          </div>

          <div class="form-group mb-4">
            <i class="fa-solid fa-lock text-muted me-2"></i>
            <label class="form-label small fw-bold text-uppercase">Password</label>
            <input v-model="form.password"
                   type="password"
                   class="form-control custom-input"
                   placeholder="Enter password"
                   required />
          </div>

          <div v-if="errorMessage" class="error-msg">
            {{ errorMessage }}
          </div>

        </form>

        <p class="register-link text-center mt-4">
          Don't have an account?
          <router-link to="/register" class="text-success fw-bold">Register here</router-link>
        </p>
      </div>
    </div>

    <div class="image-side">
      <div class="shape shape-1"></div>
      <div class="shape shape-2"></div>
      <div class="shape shape-3"></div>

      <div class="overlay-content">
        <div class="icon-box mb-4">
          <i class="fas fa-headset fa-4x text-white"></i>
        </div>
        <h3 class="display-4 fw-bold text-white custom-title ">Ticket Support System</h3>
        <p class="lead text-white-50">Manage tickets, track progress, and resolve issues faster than ever before.</p>
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

  const errorMessage = ref('');

  const form = reactive({
    username: '',
    password: ''
  });

  const handleLogin = async () => {
    errorMessage.value = '';

    try {
      const response = await api.post('/api/Account/Login', {
        username: form.username,
        password: form.password
      });

      const { token } = response.data;

      login(token);

      router.push('/app/home');

    } catch (err) {
      console.error("Login Error:", err);
      if (err.response && err.response.status === 401) {
        errorMessage.value = "Invalid username or password.";
      } else {
        errorMessage.value = "Connection error. Is the API running?";
      }
    } 
  };
</script>

<style scoped>
  /* Main Container */
  .split-container {
    display: flex;
    min-height: 100vh;
    background-color: #ffffff;
  }

  /* Left Side Styling */
  .login-side {
    flex: 1;
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 40px;
  }

  .form-wrapper {
    width: 100%;
    max-width: 400px;
  }

  .custom-input {
    padding: 12px;
    border-radius: 8px;
    border: 1px solid #e2e8f0;
    background-color: #f8fafc;
  }

    .custom-input:focus {
      border-color: #46ba86;
      box-shadow: 0 0 0 3px rgba(70, 186, 134, 0.1);
    }

  .btn-login {
    width: 100%;
    padding: 14px;
    background: linear-gradient(135deg, #46ba86 0%, #2d8a63 100%);
    color: white;
    border: none;
    border-radius: 8px;
    font-weight: 600;
    transition: all 0.3s ease;
  }

    .btn-login:hover:not(:disabled) {
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(45, 138, 99, 0.3);
    }


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


  .shape {
    position: absolute;
    filter: blur(80px);
    opacity: 0.6;
    border-radius: 50%;
    z-index: 0;
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
    width: 100px;
    height: 100px;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 20px;
    backdrop-filter: blur(10px);
    border: 1px solid rgba(255, 255, 255, 0.2);
  }

  @media (min-width: 992px) {
    .image-side {
      display: flex;
    }
  }

  .error-msg {
    color: #dc3545;
    background: #fff5f5;
    padding: 10px;
    border-radius: 6px;
    margin-bottom: 20px;
    font-size: 0.85rem;
    border: 1px solid #feb2b2;
  }

  .custom-title {
  font-size: clamp(2rem, 4vw, 3.5rem); 
  white-space: nowrap;
}
</style>
