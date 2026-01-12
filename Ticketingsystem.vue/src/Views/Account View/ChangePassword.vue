<template>
  <div class="container py-5 change-password-container">
    <div class="row justify-content-center">
      <div class="col-md-8 col-lg-6">
        <h2 class="text-center mb-5 fw-bold profile-title">Account Security</h2>

        <div class="card profile-card shadow-lg border-0 rounded-xl">
          <div class="card-body p-5">
            <h4 class="mb-4 text-muted">Update Password</h4>
            <form @submit.prevent="confirm">

              <div class="mb-4">
                <label for="oldPassword" class="form-label"><strong>Current Password</strong></label>
                <div class="input-group">
                  <input v-model="oldPassword"
                         placeholder="Enter current password"
                         type="password"
                         required
                         class="form-control password-input" />
                  <span class="input-group-text"><i class="fas fa-lock text-muted"></i></span>
                </div>
              </div>

              <hr class="my-4 text-muted opacity-25">

              <div class="mb-4">
                <label for="newPassword" class="form-label"><strong>New Password</strong></label>
                <div class="input-group">
                  <input v-model="newPassword"
                         placeholder="Minimum 8 characters"
                         type="password"
                         required
                         class="form-control password-input" />
                  <span class="input-group-text"><i class="fas fa-key text-muted"></i></span>
                </div>
              </div>

              <div class="mb-5">
                <label for="confirmPassword" class="form-label"><strong>Confirm New Password</strong></label>
                <div class="input-group">
                  <input v-model="confirmPassword"
                         placeholder="Repeat new password"
                         type="password"
                         required
                         class="form-control password-input" />
                  <span class="input-group-text"><i class="fas fa-check-circle text-muted"></i></span>
                </div>
              </div>

              <button type="submit"
                      :disabled="loading"
                      class="btn update-btn w-100 shadow-sm">
                <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
                {{ loading ? 'Updating...' : 'Change Password' }}
              </button>

            </form>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref } from 'vue'
  import { useRouter } from 'vue-router'
  import api from '@/components/Authentication Service/AuthAPI'
  import { useConfirmWarning, successDialog, errorDialog } from '@/components/Modals/Modal'
  import { useAuth } from '@/components/Authentication Service/Authentication'

  // State
  const loading = ref(false)
  const oldPassword = ref('')
  const newPassword = ref('')
  const confirmPassword = ref('')

  const router = useRouter()
  const confirmDialogWarning = useConfirmWarning().confirmDialog
  const { logout: authLogout } = useAuth()


  const confirm = async () => {
    if (newPassword.value !== confirmPassword.value) {
      await errorDialog('Passwords do not match.')
      return
    }
    if (newPassword.value === oldPassword.value) {
      await errorDialog('The new password must be different from the old one.')
      return
    }
    if (newPassword.value.length < 6) {
      await errorDialog('Password is too short.')
      return
    }

    const confirmed = await confirmDialogWarning(
      'Security Update',
      'You will be logged out and asked to sign in with your new password.',
      'Change'
    )

    if (confirmed) {
      await executeChange()
    }
  }

  const executeChange = async () => {
    loading.value = true

    const payload = {
      CurrentPassword: oldPassword.value,
      NewPassword: newPassword.value
    };

    try {
      await api.patch(`/api/Account/ChangePassword`, payload)

      await successDialog('Success! Please log in with your new password.')

      await authLogout()
      router.push('/')
    }
    catch (err) {
      let msg = 'Update failed. Check your current password and try again.';
      if (err.response && err.response.data) {
        msg = err.response.data; 
      }
      await errorDialog(msg)
    } finally {
      loading.value = false
    }
  }
</script>

<style scoped>
  .change-password-container {
    background-color: #f4f7f6;
    min-height: 100vh;
  }

  .profile-title {
    color: #2c3e50;
    letter-spacing: -1px;
  }

  .profile-card {
    border-radius: 1.25rem;
    overflow: hidden;
  }

  .form-label {
    font-size: 0.9rem;
    color: #4a5568;
  }

  /* Specific styling for the password inputs */
  .password-input {
    border-right: none;
    padding: 0.8rem 1rem;
    border-color: #d1d5db;
  }

  .input-group-text {
    background-color: white;
    border-left: none;
    border-color: #d1d5db;
    color: #a0aec0;
  }

  /* Interaction States */
  .form-control:focus {
    border-color: #46ba86;
    box-shadow: 0 0 0 3px rgba(70, 186, 134, 0.1);
  }

    .form-control:focus + .input-group-text {
      border-color: #46ba86;
    }

  .update-btn {
    background-color: #46ba86;
    border: none;
    color: white;
    padding: 0.9rem;
    border-radius: 0.75rem;
    font-weight: 700;
    transition: transform 0.2s, background-color 0.2s;
  }

    .update-btn:hover:not(:disabled) {
      background-color: #3da678;
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(70, 186, 134, 0.3);
    }

    .update-btn:disabled {
      opacity: 0.7;
      cursor: not-allowed;
    }
</style>
