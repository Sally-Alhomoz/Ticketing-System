<template>
  <div class="language-toggle-fixed">
    <button @click="toggleLanguage" class="btn btn-dark shadow-lg rounded-pill px-4">
      <i class="fas fa-globe me-2"></i>
      {{ locale === 'en' ? 'العربية' : 'English' }}
    </button>
  </div>

  <router-view />
</template>

<script setup>
import { useI18n } from 'vue-i18n'
const { locale } = useI18n()

const toggleLanguage = () => {
  const newLang = locale.value === 'en' ? 'ar' : 'en'
  locale.value = newLang
  document.documentElement.dir = newLang === 'ar' ? 'rtl' : 'ltr'
  document.documentElement.lang = newLang
  localStorage.setItem('user_lang', newLang)
}
</script>

<style scoped>
  .language-toggle-fixed {
    position: fixed;
    top: 20px;
    right: 20px;
    z-index: 9999; 
  }

  [dir="rtl"] .language-toggle-fixed {
    right: auto;
    left: 20px;
  }

  .btn-dark {
    background-color: #2c3e50;
    border: none;
    transition: all 0.3s ease;
  }

    .btn-dark:hover {
      transform: scale(1.05);
      background-color: #46ba86; 
    }
</style>
