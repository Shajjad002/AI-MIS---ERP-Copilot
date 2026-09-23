<script setup lang="ts">
defineProps<{ userName: string | null }>()
const emit = defineEmits<{ openVisualization: [] }>()
</script>

<template>
  <main class="shell home-shell">
    <section class="home-hero">
      <p class="eyebrow">AI MIS & ERP Copilot</p>
      <h1>Good to see you{{ userName ? `, ${userName}` : '' }}.</h1>
      <p class="lede">Turn everyday ERP questions into secure, readable business insights.</p>
      <button type="button" @click="emit('openVisualization')">Open Visualization</button>
    </section>
    <section class="home-grid">
      <article class="home-card">
        <span class="home-icon">01</span>
        <h2>Ask in plain language</h2>
        <p>Describe the MIS information you need without writing SQL.</p>
      </article>
      <article class="home-card">
        <span class="home-icon">02</span>
        <h2>Review the result</h2>
        <p>See summaries, KPIs, charts, and the underlying result table.</p>
      </article>
      <article class="home-card">
        <span class="home-icon">03</span>
        <h2>Stay in control</h2>
        <p>Queries are validated as read-only before they reach approved data.</p>
      </article>
    </section>
  </main>
</template>
