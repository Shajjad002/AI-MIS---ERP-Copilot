<script setup lang="ts">
const features = ['Secure access foundation', 'Role and branch access model', 'API health and system status']
</script>

<template>
  <main>
    <p class="eyebrow">AI MIS & ERP Copilot</p>
    <h1>Foundation is ready.</h1>
    <p>The workspace is prepared for secure ERP/MIS analytics. AI querying is intentionally not enabled yet.</p>
    <section><h2>Sprint 1</h2><ul><li v-for="feature in features" :key="feature">{{ feature }}</li></ul></section>
  </main>
</template>
