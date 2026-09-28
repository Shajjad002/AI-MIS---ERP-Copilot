<script setup lang="ts">
import { computed, ref } from 'vue'
import HomePage from './pages/HomePage.vue'
import LoginPage from './pages/LoginPage.vue'
import VisualizationPage from './pages/VisualizationPage.vue'
import UserAccessPage from './pages/UserAccessPage.vue'
import CreateUserPage from './pages/CreateUserPage.vue'
import DocumentsPage from './pages/DocumentsPage.vue'
import { useAuth } from './composables/useAuth'

const { token, userName, roles, logout } = useAuth()
const authenticated = ref(Boolean(token.value))
const currentPage = ref<'home' | 'visualization' | 'users' | 'create-user' | 'documents'>('home')
const isAdministrator = computed(() => roles.value.includes('Administrator'))
const canManageDocuments = computed(() => roles.value.includes('Administrator') || roles.value.includes('MIS Analyst'))

function signIn() {
  authenticated.value = true
  token.value = localStorage.getItem('ai-mis.access-token')
  userName.value = localStorage.getItem('ai-mis.user')
  try {
    roles.value = JSON.parse(localStorage.getItem('ai-mis.roles') ?? '[]')
  } catch {
    roles.value = []
  }
  currentPage.value = 'home'
}
</script>

<template>
  <LoginPage v-if="!authenticated" @authenticated="signIn" />
  <template v-else>
    <div class="app-layout">
      <nav class="app-sidebar">
        <span class="brand">AI MIS & ERP Copilot</span>
        <div class="sidebar-links">
          <button class="nav-link" :class="{ active: currentPage === 'home' }" type="button" @click="currentPage = 'home'">Home</button>
          <button class="nav-link" :class="{ active: currentPage === 'visualization' }" type="button" @click="currentPage = 'visualization'">Visualization</button>
          <button v-if="isAdministrator" class="nav-link" :class="{ active: currentPage === 'users' }" type="button" @click="currentPage = 'users'">Users</button>
          <button v-if="isAdministrator" class="nav-link" :class="{ active: currentPage === 'create-user' }" type="button" @click="currentPage = 'create-user'">Create user</button>
          <button v-if="canManageDocuments" class="nav-link" :class="{ active: currentPage === 'documents' }" type="button" @click="currentPage = 'documents'">Documents</button>
        </div>
        <div class="sidebar-footer">
          <span class="signed-in">Signed in as {{ userName }}</span>
          <button class="secondary-button" type="button" @click="logout(); authenticated = false; currentPage = 'home'">Sign out</button>
        </div>
      </nav>
      <div class="app-content">
        <HomePage v-if="currentPage === 'home'" :user-name="userName" @open-visualization="currentPage = 'visualization'" />
        <VisualizationPage v-else-if="currentPage === 'visualization'" />
        <UserAccessPage v-else-if="currentPage === 'users'" @create-user="currentPage = 'create-user'" />
        <CreateUserPage v-else-if="currentPage === 'create-user'" @back="currentPage = 'users'" />
        <DocumentsPage v-else />
      </div>
    </div>
  </template>
</template>
