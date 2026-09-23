<script setup lang="ts">
import { computed } from 'vue'

defineProps<{ userName: string | null }>()
const emit = defineEmits<{ openVisualization: [] }>()

const trendValues = [42, 57, 49, 68, 64, 82, 91]
const trendLabels = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']
const categoryValues = [
  { label: 'Sales', value: 86, color: '#67e8f9' },
  { label: 'Inventory', value: 68, color: '#818cf8' },
  { label: 'Receivables', value: 52, color: '#f0abfc' },
  { label: 'Orders', value: 74, color: '#86efac' },
]
const trendPoints = computed(() => trendValues.map((value, index) => `${index * 62 + 10},${108 - value}`).join(' '))
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
    <section class="home-dashboard" aria-labelledby="dashboard-title">
      <div class="dashboard-heading">
        <div>
          <p class="eyebrow">Live-ready overview</p>
          <h2 id="dashboard-title">Business snapshot</h2>
        </div>
        <button class="secondary-button" type="button" @click="emit('openVisualization')">Explore data</button>
      </div>
      <div class="dashboard-kpis">
        <article class="dashboard-kpi">
          <span>Revenue trend</span>
          <strong>+18.6%</strong>
          <small>Compared with last week</small>
        </article>
        <article class="dashboard-kpi">
          <span>Orders processed</span>
          <strong>1,284</strong>
          <small>Across approved branches</small>
        </article>
        <article class="dashboard-kpi">
          <span>Collection rate</span>
          <strong>84.2%</strong>
          <small>Current reporting period</small>
        </article>
      </div>
      <div class="dashboard-charts">
        <article class="dashboard-chart">
          <div class="card-heading">
            <div><h3>Weekly activity</h3><span>Demo trend preview</span></div>
            <strong class="chart-highlight">91</strong>
          </div>
          <div class="line-chart" role="img" aria-label="Weekly activity trend rising from 42 to 91">
            <div class="chart-gridline chart-gridline-high" />
            <div class="chart-gridline chart-gridline-mid" />
            <div class="chart-gridline chart-gridline-low" />
            <svg viewBox="0 0 390 120" preserveAspectRatio="none" aria-hidden="true">
              <polygon :points="`10,108 ${trendPoints} 382,108`" class="chart-area" />
              <polyline :points="trendPoints" class="chart-line" />
              <circle v-for="(value, index) in trendValues" :key="value" :cx="index * 62 + 10" :cy="108 - value" r="3.5" class="chart-dot" />
            </svg>
            <div class="chart-labels">
              <span v-for="label in trendLabels" :key="label">{{ label }}</span>
            </div>
          </div>
        </article>
        <article class="dashboard-chart">
          <div class="card-heading"><div><h3>Area performance</h3><span>Relative contribution</span></div></div>
          <div class="home-bars">
            <div v-for="item in categoryValues" :key="item.label" class="home-bar-row">
              <span>{{ item.label }}</span>
              <span class="home-bar-track"><span class="home-bar-fill" :style="{ width: `${item.value}%`, background: item.color }" /></span>
              <strong>{{ item.value }}%</strong>
            </div>
          </div>
        </article>
      </div>
      <p class="dashboard-note">Preview metrics are illustrative. Run a query in Visualization to see your approved ERP data.</p>
    </section>
  </main>
</template>
