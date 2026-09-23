<script setup lang="ts">
import { ref } from 'vue'
import HomePage from './pages/HomePage.vue'
import LoginPage from './pages/LoginPage.vue'
import VisualizationPage from './pages/VisualizationPage.vue'
import { useAuth } from './composables/useAuth'

const { token, userName, logout } = useAuth()
const authenticated = ref(Boolean(token.value))
const currentPage = ref<'home' | 'visualization'>('home')

function signIn() {
  authenticated.value = true
  userName.value = localStorage.getItem('ai-mis.user')
  currentPage.value = 'home'
}
</script>

<template>
  <LoginPage v-if="!authenticated" @authenticated="signIn" />
  <template v-else>
    <nav class="app-nav">
      <span class="brand">AI MIS & ERP Copilot</span>
      <button class="nav-link" type="button" @click="currentPage = 'home'">Home</button>
      <button class="nav-link" type="button" @click="currentPage = 'visualization'">Visualization</button>
      <span class="signed-in">Signed in as {{ userName }}</span>
      <button class="secondary-button" type="button" @click="logout(); authenticated = false; currentPage = 'home'">Sign out</button>
    </nav>
    <HomePage v-if="currentPage === 'home'" :user-name="userName" @open-visualization="currentPage = 'visualization'" />
    <VisualizationPage v-else />
  </template>
</template>
