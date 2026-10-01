<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref, watch } from 'vue'
import HomePage from './pages/HomePage.vue'
import LoginPage from './pages/LoginPage.vue'
import VisualizationPage from './pages/VisualizationPage.vue'
import UserAccessPage from './pages/UserAccessPage.vue'
import CreateUserPage from './pages/CreateUserPage.vue'
import DocumentsPage from './pages/DocumentsPage.vue'
import { useAuth } from './composables/useAuth'

type Page = 'home' | 'visualization' | 'users' | 'create-user' | 'documents'
type NavigationPage = Exclude<Page, 'create-user'>
type NavigationItem = { page: NavigationPage; label: string; icon: 'dashboard' | 'chart' | 'documents' | 'users' }

const { token, userName, roles, logout } = useAuth()
const authenticated = ref(Boolean(token.value))
const currentPage = ref<Page>('home')
const sidebarCollapsed = ref(false)
const mobileMenuOpen = ref(false)
const isMobileViewport = ref(window.matchMedia('(max-width: 760px)').matches)
const profileMenuOpen = ref(false)
const notificationsOpen = ref(false)
const isDark = ref(localStorage.getItem('ai-mis.theme') === 'dark')
const searchQuery = ref('')
const searchOpen = ref(false)
const searchInput = ref<HTMLInputElement | null>(null)

const isAdministrator = computed(() => roles.value.includes('Administrator'))
const canManageDocuments = computed(() => isAdministrator.value || roles.value.includes('MIS Analyst'))
const navigationGroups = computed(() => {
  const workspace: NavigationItem[] = [
    { page: 'home', label: 'Dashboard', icon: 'dashboard' },
    { page: 'visualization', label: 'MIS Analytics', icon: 'chart' },
  ]
  const knowledge: NavigationItem[] = []
  const administration: NavigationItem[] = []
  if (canManageDocuments.value) knowledge.push({ page: 'documents', label: 'Documents', icon: 'documents' })
  if (isAdministrator.value) administration.push({ page: 'users', label: 'User access', icon: 'users' })
  return [
    { label: 'Workspace', items: workspace },
    ...(knowledge.length ? [{ label: 'Knowledge', items: knowledge }] : []),
    ...(administration.length ? [{ label: 'Administration', items: administration }] : []),
  ]
})
const navigationItems = computed(() => navigationGroups.value.flatMap(group => group.items))
const searchResults = computed(() => {
  const query = searchQuery.value.trim().toLowerCase()
  if (!query) return []
  return navigationItems.value.filter(item => item.label.toLowerCase().includes(query)).slice(0, 5)
})
const pageTitle = computed(() => ({
  home: 'Dashboard',
  visualization: 'MIS Analytics',
  users: 'User access',
  'create-user': 'Create user',
  documents: 'Documents & business rules',
}[currentPage.value]))
const userInitial = computed(() => userName.value?.trim().charAt(0).toUpperCase() || 'U')

watch(isDark, dark => {
  document.documentElement.dataset.theme = dark ? 'dark' : 'light'
  localStorage.setItem('ai-mis.theme', dark ? 'dark' : 'light')
}, { immediate: true })

function updateViewport() {
  isMobileViewport.value = window.matchMedia('(max-width: 760px)').matches
  if (!isMobileViewport.value) mobileMenuOpen.value = false
}

onMounted(() => window.addEventListener('resize', updateViewport))
onUnmounted(() => window.removeEventListener('resize', updateViewport))

function navigate(page: Page) {
  currentPage.value = page
  mobileMenuOpen.value = false
  profileMenuOpen.value = false
  notificationsOpen.value = false
  searchOpen.value = false
  searchQuery.value = ''
}

function signIn() {
  authenticated.value = true
  token.value = localStorage.getItem('ai-mis.access-token')
  userName.value = localStorage.getItem('ai-mis.user')
  try {
    roles.value = JSON.parse(localStorage.getItem('ai-mis.roles') ?? '[]')
  } catch {
    roles.value = []
  }
  navigate('home')
}

function signOut() {
  logout()
  authenticated.value = false
  navigate('home')
}

function closeMenus(event: KeyboardEvent) {
  if (event.key === 'Escape') {
    mobileMenuOpen.value = false
    profileMenuOpen.value = false
    notificationsOpen.value = false
    searchOpen.value = false
  }
}

function toggleNavigation() {
  if (isMobileViewport.value) mobileMenuOpen.value = !mobileMenuOpen.value
  else sidebarCollapsed.value = !sidebarCollapsed.value
}

function focusSearch() {
  searchInput.value?.focus()
}

function selectFirstSearchResult() {
  const firstResult = searchResults.value[0]
  if (firstResult) navigate(firstResult.page)
  else searchOpen.value = false
}
</script>

<template>
  <LoginPage v-if="!authenticated" @authenticated="signIn" />
  <div
    v-else
    class="app-layout"
    :class="{
      'sidebar-collapsed': sidebarCollapsed,
      'mobile-menu-open': mobileMenuOpen,
    }"
    @keydown="closeMenus"
    @keydown.ctrl.k.prevent="focusSearch"
    @keydown.meta.k.prevent="focusSearch"
  >
    <button
      v-if="mobileMenuOpen"
      class="sidebar-scrim"
      type="button"
      aria-label="Close navigation"
      @click="mobileMenuOpen = false"
    />

    <aside class="app-sidebar" aria-label="Main navigation">
      <div class="sidebar-brand-row">
        <a class="brand" href="#" aria-label="AI MIS Copilot dashboard" @click.prevent="navigate('home')">
          <span class="brand-mark" aria-hidden="true">
            <svg viewBox="0 0 24 24" fill="none"><path d="M6 17V9m6 8V5m6 12v-5" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" /></svg>
          </span>
          <span class="brand-name">MIS Copilot</span>
        </a>
        <button class="icon-button sidebar-close" type="button" aria-label="Close navigation" @click="mobileMenuOpen = false">
          <svg viewBox="0 0 24 24" fill="none" aria-hidden="true"><path d="m6 6 12 12M18 6 6 18" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" /></svg>
        </button>
      </div>

      <nav class="sidebar-navigation">
        <section v-for="group in navigationGroups" :key="group.label" class="sidebar-group">
          <h2 class="sidebar-section-label">{{ group.label }}</h2>
          <button
            v-for="item in group.items"
            :key="item.page"
            class="nav-link"
            :class="{ active: currentPage === item.page || (item.page === 'users' && currentPage === 'create-user') }"
            type="button"
            :title="sidebarCollapsed ? item.label : undefined"
            :aria-current="currentPage === item.page || (item.page === 'users' && currentPage === 'create-user') ? 'page' : undefined"
            @click="navigate(item.page)"
          >
            <svg v-if="item.icon === 'dashboard'" viewBox="0 0 24 24" fill="none" aria-hidden="true">
              <rect x="4" y="4" width="6" height="6" rx="1.2" stroke="currentColor" stroke-width="1.7" />
              <rect x="14" y="4" width="6" height="6" rx="1.2" stroke="currentColor" stroke-width="1.7" />
              <rect x="4" y="14" width="6" height="6" rx="1.2" stroke="currentColor" stroke-width="1.7" />
              <rect x="14" y="14" width="6" height="6" rx="1.2" stroke="currentColor" stroke-width="1.7" />
            </svg>
            <svg v-else-if="item.icon === 'chart'" viewBox="0 0 24 24" fill="none" aria-hidden="true">
              <path d="M4 19.5h16M6.5 16V10m5 6V5m5 11v-8" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" />
            </svg>
            <svg v-else-if="item.icon === 'documents'" viewBox="0 0 24 24" fill="none" aria-hidden="true">
              <path d="M7 3.8h7l4 4V20H7a2 2 0 0 1-2-2V5.8a2 2 0 0 1 2-2Z" stroke="currentColor" stroke-width="1.7" stroke-linejoin="round" />
              <path d="M14 4v4h4M9 12h6m-6 3.5h6" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" />
            </svg>
            <svg v-else viewBox="0 0 24 24" fill="none" aria-hidden="true">
              <circle cx="9" cy="8" r="3.3" stroke="currentColor" stroke-width="1.7" />
              <path d="M3.5 19a5.5 5.5 0 0 1 11 0m1-12.5a3.3 3.3 0 0 1 0 6.3M17 14a5 5 0 0 1 3.5 4.8" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" />
            </svg>
            <span class="nav-label">{{ item.label }}</span>
          </button>
        </section>
      </nav>

      <div class="sidebar-footer">
        <span class="signed-in">{{ userName }}</span>
        <span class="sidebar-role">{{ roles[0] || 'User' }}</span>
      </div>
    </aside>

    <div class="app-content">
      <header class="topbar">
        <button
          class="icon-button menu-toggle"
          type="button"
          :aria-label="isMobileViewport ? mobileMenuOpen ? 'Close navigation' : 'Open navigation' : sidebarCollapsed ? 'Expand navigation' : 'Collapse navigation'"
          @click="toggleNavigation"
        >
          <svg viewBox="0 0 24 24" fill="none" aria-hidden="true"><path d="M4 6.5h16M4 12h16M4 17.5h16" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" /></svg>
        </button>

        <div class="topbar-search">
          <svg class="search-icon" viewBox="0 0 24 24" fill="none" aria-hidden="true"><circle cx="10.8" cy="10.8" r="6.8" stroke="currentColor" stroke-width="1.7" /><path d="m16 16 4 4" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" /></svg>
          <input
            ref="searchInput"
            v-model="searchQuery"
            type="search"
            aria-label="Search pages"
            placeholder="Search pages..."
            @focus="searchOpen = true"
            @keydown.enter.prevent="selectFirstSearchResult"
          />
          <kbd>⌘ K</kbd>
          <div v-if="searchOpen && searchQuery.trim()" class="search-results">
            <button v-for="item in searchResults" :key="item.page" type="button" @click="navigate(item.page)">
              <span>{{ item.label }}</span><span class="search-result-hint">Open page</span>
            </button>
            <p v-if="searchResults.length === 0">No matching pages</p>
          </div>
        </div>

        <div class="topbar-actions">
          <button
            class="icon-button"
            type="button"
            :aria-label="isDark ? 'Switch to light theme' : 'Switch to dark theme'"
            :title="isDark ? 'Switch to light theme' : 'Switch to dark theme'"
            @click="isDark = !isDark"
          >
            <svg v-if="!isDark" viewBox="0 0 24 24" fill="none" aria-hidden="true"><path d="M20.2 15.2A8.5 8.5 0 0 1 8.8 3.8 8.6 8.6 0 1 0 20.2 15.2Z" stroke="currentColor" stroke-width="1.7" stroke-linejoin="round" /></svg>
            <svg v-else viewBox="0 0 24 24" fill="none" aria-hidden="true"><circle cx="12" cy="12" r="4" stroke="currentColor" stroke-width="1.7" /><path d="M12 2v2m0 16v2M4.93 4.93l1.42 1.42m11.3 11.3 1.42 1.42M2 12h2m16 0h2M4.93 19.07l1.42-1.42m11.3-11.3 1.42-1.42" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" /></svg>
          </button>

          <div class="topbar-menu-wrap">
            <button class="icon-button notification-button" type="button" aria-label="Notifications" :aria-expanded="notificationsOpen" @click="notificationsOpen = !notificationsOpen; profileMenuOpen = false">
              <svg viewBox="0 0 24 24" fill="none" aria-hidden="true"><path d="M18 9a6 6 0 0 0-12 0c0 7-3 7-3 9h18c0-2-3-2-3-9Zm-8 12h4" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round" /></svg>
              <span class="notification-dot" />
            </button>
            <div v-if="notificationsOpen" class="topbar-popover notification-popover">
              <strong>Notifications</strong>
              <p>You’re all caught up.</p>
            </div>
          </div>

          <div class="topbar-menu-wrap">
            <button class="profile-button" type="button" :aria-expanded="profileMenuOpen" @click="profileMenuOpen = !profileMenuOpen; notificationsOpen = false">
              <span class="profile-avatar" aria-hidden="true">{{ userInitial }}</span>
              <span class="profile-details"><strong>{{ userName }}</strong><small>{{ roles[0] || 'User' }}</small></span>
              <svg class="profile-chevron" viewBox="0 0 24 24" fill="none" aria-hidden="true"><path d="m7 10 5 5 5-5" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" /></svg>
            </button>
            <div v-if="profileMenuOpen" class="topbar-popover profile-popover">
              <div class="profile-popover-user"><strong>{{ userName }}</strong><span>{{ roles[0] || 'User' }}</span></div>
              <button type="button" @click="signOut">
                <svg viewBox="0 0 24 24" fill="none" aria-hidden="true"><path d="M10 17l5-5-5-5m5 5H3m9-9h6a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2h-6" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round" /></svg>
                Sign out
              </button>
            </div>
          </div>
        </div>
      </header>

      <main class="page-content">
        <div class="content-heading"><span>Workspace</span><span aria-hidden="true">/</span><strong>{{ pageTitle }}</strong></div>
        <HomePage v-if="currentPage === 'home'" :user-name="userName" @open-visualization="navigate('visualization')" />
        <VisualizationPage v-else-if="currentPage === 'visualization'" />
        <UserAccessPage v-else-if="currentPage === 'users'" @create-user="navigate('create-user')" />
        <CreateUserPage v-else-if="currentPage === 'create-user'" @back="navigate('users')" />
        <DocumentsPage v-else />
      </main>
    </div>
  </div>
</template>
