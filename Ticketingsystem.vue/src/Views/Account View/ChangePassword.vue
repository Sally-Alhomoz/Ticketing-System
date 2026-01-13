<template>
  <div class="container py-5 change-password-container" :dir="locale === 'ar' ? 'rtl' : 'ltr'">
    <div class="row justify-content-center">
      <div class="col-md-8 col-lg-6">

        <h2 class="text-center mb-5 fw-bold profile-title">{{ $t('changePassword.title') }}</h2>

        <div class="card profile-card shadow-lg border-0 rounded-xl">
          <div class="card-body p-5">
            <h4 class="mb-4 text-muted border-bottom pb-2">{{ $t('changePassword.subtitle') }}</h4>
            <form @submit.prevent="confirm">

              <div class="mb-4">
                <label for="oldPassword" class="form-label"><strong>{{ $t('changePassword.current') }}</strong></label>
                <div class="input-group">
                  <input v-model="oldPassword"
                         :placeholder="$t('changePassword.placeholders.current')"
                         type="password"
                         required
                         class="form-control password-input" />
                  <span class="input-group-text"><i class="fas fa-lock text-muted"></i></span>
                </div>
              </div>

              <hr class="my-4 text-muted opacity-25">

              <div class="mb-4">
                <label for="newPassword" class="form-label"><strong>{{ $t('changePassword.new') }}</strong></label>
                <div class="input-group">
                  <input v-model="newPassword"
                         :placeholder="$t('changePassword.placeholders.new')"
                         type="password"
                         required
                         class="form-control password-input" />
                  <span class="input-group-text"><i class="fas fa-key text-muted"></i></span>
                </div>
              </div>

              <div class="mb-5">
                <label for="confirmPassword" class="form-label"><strong>{{ $t('changePassword.confirm') }}</strong></label>
                <div class="input-group">
                  <input v-model="confirmPassword"
                         :placeholder="$t('changePassword.placeholders.confirm')"
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
                {{ loading ? $t('changePassword.loading') : $t('changePassword.button') }}
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
  import { useI18n } from 'vue-i18n' // Import i18n
  import api from '@/components/Authentication Service/AuthAPI'
  import { useConfirmWarning, successDialog, errorDialog } from '@/components/Modals/Modal'
  import { useAuth } from '@/components/Authentication Service/Authentication'

  const { t, locale } = useI18n()
  const loading = ref(false)
  const oldPassword = ref('')
  const newPassword = ref('')
  const confirmPassword = ref('')

  const router = useRouter()
  const confirmDialogWarning = useConfirmWarning().confirmDialog
  const { logout: authLogout } = useAuth()


  const confirm = async () => {
    if (newPassword.value !== confirmPassword.value) {
      await errorDialog(t('changePassword.errors.match'))
      return
    }
    if (newPassword.value.length < 6) {
      await errorDialog(t('changePassword.errors.short'))
      return
    }

    const confirmed = await confirmDialogWarning(
      t('changePassword.modal.title'),
      t('changePassword.modal.body'),
      t('changePassword.modal.confirm')
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
      await successDialog(t('changePassword.success'))
      await authLogout()
      router.push('/')
    }
    catch (err) {
      let msg = t('changePassword.errors.failed');
      if (err.response && err.response.data) { msg = err.response.data; }
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

  [dir="rtl"] {
    font-family: 'Cairo', sans-serif;
  }

  .profile-title {
    color: #2c3e50;
    letter-spacing: -0.5px;
  }

  .profile-card {
    border-radius: 1.25rem;
  }

  .password-input {
    border-inline-end: none;
    padding: 0.8rem 1rem;
    border-color: #d1d5db;
  }

  .input-group-text {
    background-color: white;
    border-inline-start: none;
    border-color: #d1d5db;
  }

  .update-btn {
    background-color: #46ba86;
    border: none;
    color: white;
    padding: 0.9rem;
    border-radius: 0.75rem;
    font-weight: 700;
  }

    .update-btn:hover:not(:disabled) {
      background-color: #3da678;
      transform: translateY(-2px);
    }
</style>
