<template>
  <div class="container py-5 edit-profile-container">
    <h2 class="text-center mb-5 fw-bold profile-title">Profile Settings</h2>

    <div class="card profile-card shadow-lg border-0 rounded-xl">
      <div class="card-body p-4 p-md-5">
        <div class="row">
          <div class="col-lg-4 border-end pe-lg-4 mb-4 mb-lg-0 text-center">
            <div class="avatar-placeholder mb-3">
              <div class="avatar-circle">
                {{ firstName?.[0] }}{{ lastName?.[0] }}
              </div>
            </div>
            <h4 class="fw-bold">{{ firstName }} {{ lastName }}</h4>
            <p class="text-muted">@{{ username }}</p>

            <div class="mt-4 pt-3 border-top">
              <router-link to="/app/profile/changepassword" class="btn btn-outline-secondary w-100">
                <i class="fas fa-lock me-2"></i> Change Password
              </router-link>
            </div>
          </div>

          <div class="col-lg-8 ps-lg-4">
            <div class="d-flex justify-content-between align-items-center mb-4">
              <h5 class="fw-semibold text-dark mb-0">Account Information</h5>
              <button class="btn btn-sm"
                      :class="isEditing ? 'btn-danger' : 'btn-outline-primary'"
                      @click="toggleEdit">
                <i :class="isEditing ? 'fas fa-times' : 'fas fa-edit'"></i>
                {{ isEditing ? 'Cancel' : 'Edit Profile' }}
              </button>
            </div>

            <form @submit.prevent="confirmUpdate">
              <div class="mb-4">
                <label class="form-label text-dark"><strong>Username</strong></label>
                <div class="input-group">
                  <input type="text" class="form-control bg-light" v-model="username" readonly />
                  <span class="input-group-text"><i class="fas fa-at text-muted"></i></span>
                </div>
              </div>

              <div class="row">
                <div class="col-md-6 mb-4">
                  <label class="form-label text-dark"><strong>First Name</strong></label>
                  <input type="text" class="form-control" v-model="firstName" :readonly="!isEditing" :class="{'is-editing': isEditing}" />
                </div>
                <div class="col-md-6 mb-4">
                  <label class="form-label text-dark"><strong>Last Name</strong></label>
                  <input type="text" class="form-control" v-model="lastName" :readonly="!isEditing" :class="{'is-editing': isEditing}" />
                </div>
              </div>

              <div class="mb-4">
                <label class="form-label text-dark"><strong>E-mail</strong></label>
                <div class="input-group">
                  <input type="email" class="form-control" v-model="email" :readonly="!isEditing" :class="{'is-editing': isEditing}" />
                  <span class="input-group-text"><i class="fas fa-envelope text-muted"></i></span>
                </div>
              </div>

              <div class="d-flex justify-content-end mt-5" v-if="isEditing">
                <button type="submit" class="btn btn-primary update-btn">
                  Save Changes
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted } from 'vue'
  import api from '@/components/Authentication Service/AuthAPI'
  import { useConfirmWarning, successDialog, errorDialog } from '@/components/Modals/Modal'

  const username = ref('')
  const email = ref('')
  const firstName = ref('')
  const lastName = ref('')

  const originalData = ref({})

  const isEditing = ref(false)
  const { confirmDialog } = useConfirmWarning()

  const toggleEdit = () => {
    if (isEditing.value) {
      email.value = originalData.value.email
      firstName.value = originalData.value.firstName
      lastName.value = originalData.value.lastName
    } else {
      originalData.value = {
        email: email.value,
        firstName: firstName.value,
        lastName: lastName.value
      }
    }
    isEditing.value = !isEditing.value
  }

  const confirmUpdate = async () => {
    const confirmed = await confirmDialog(
      'Save Changes?',
      'Are you sure you want to update your profile information?',
      'Save'
    )
    if (confirmed) {
      await updateProfile()
    }
  }

  const updateProfile = async () => {
    try {
      const payload = {
        FirstName: firstName.value,
        LastName: lastName.value,
        Email: email.value
      };

      await api.put(`/api/Account/UpdateProfile`, payload);

      isEditing.value = false;
      originalData.value = { ...payload };

      await successDialog('Profile Updated', 'Your information has been successfully updated.');
    } catch (error) {
      console.error("Backend Validation Error:", error.response?.data);
      const msg = error.response?.data?.errors
        ? "Please check your input format."
        : (error.response?.data || 'Issue updating profile.');

      await errorDialog(msg, 'Update Failed');
    }
  }

  const getInfo = async () => {
    try {
      const response = await api.get(`/api/Account/GetAccountInfo`)
      username.value = response.data.username
      email.value = response.data.email
      firstName.value = response.data.firstName
      lastName.value = response.data.lastName
    } catch (err) {
      console.error("Error fetching info:", err)
    }
  }

  onMounted(() => {
    getInfo()
  })
</script>

<style scoped>
  .avatar-circle {
    width: 100px;
    height: 100px;
    background-color: #46ba86;
    color: white;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 2.5rem;
    font-weight: bold;
    margin: 0 auto;
    text-transform: uppercase;
  }

  .edit-profile-container {
    background-color: #fff;
    min-height: 100vh;
  }

  .form-control.is-editing {
    background-color: #fff;
    border-color: #46ba86;
  }

  .update-btn {
    background-color: #46ba86;
    border-color: #46ba86;
    padding: 0.75rem 2.5rem;
  }

</style>
