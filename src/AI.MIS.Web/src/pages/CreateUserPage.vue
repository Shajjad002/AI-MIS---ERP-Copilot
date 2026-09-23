<script setup lang="ts">
import { ref } from 'vue'
const emit = defineEmits<{ back: [] }>()
const form = ref({ userName: '', displayName: '', email: '', password: '', role: 'MIS Analyst', branchCodes: '' })
const message = ref('')
const error = ref('')
async function create() {
  error.value = ''; message.value = ''
  const response = await fetch('http://localhost:5252/api/users', { method: 'POST', headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${localStorage.getItem('ai-mis.access-token')}` }, body: JSON.stringify({ ...form.value, branchCodes: form.value.branchCodes.split(',').map(value => value.trim()).filter(Boolean) }) })
  const body = await response.json().catch(() => ({}))
  if (!response.ok) { error.value = body.message ?? `User creation failed (HTTP ${response.status}).`; return }
  message.value = `User ${form.value.userName} was created successfully.`
  form.value = { userName: '', displayName: '', email: '', password: '', role: 'MIS Analyst', branchCodes: '' }
}
</script>

<template>
  <main class="shell admin-shell"><div class="page-heading"><div><p class="eyebrow">Administration</p><h1>Create user</h1><p class="lede">Create an account and assign its initial access scope.</p></div><button class="secondary-button" type="button" @click="emit('back')">Back to users</button></div><form class="query-card user-form" @submit.prevent="create"><label>Username<input v-model="form.userName" required autocomplete="off" /></label><label>Display name<input v-model="form.displayName" required /></label><label>Email<input v-model="form.email" required type="email" /></label><label>Temporary password<input v-model="form.password" required minlength="8" type="password" /></label><label>Role<select v-model="form.role"><option>Administrator</option><option>MIS Analyst</option><option>Branch User</option></select></label><label>Branch codes <span class="hint">(required for Branch User; comma separated)</span><input v-model="form.branchCodes" placeholder="0212, 0215" /></label><button type="submit">Create user</button><p v-if="message" class="notice">{{ message }}</p><p v-if="error" class="error">{{ error }}</p></form></main>
</template>
