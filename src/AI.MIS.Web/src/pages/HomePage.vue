<script setup lang="ts">
import { computed, ref } from 'vue'

const props = defineProps<{ userName: string | null }>()
const emit = defineEmits<{ openVisualization: [] }>()
const chartPeriod = ref('Daily')
const chartMode = ref('line')

const topCards = [
  { label: 'Total Revenue', value: '$2,480,539', delta: '↑ 18.6% vs Apr 1 - Apr 30', tone: 'orange' },
  { label: 'Users', value: '1,234,333', delta: '↑ 12.4% vs Apr 1 - Apr 30', tone: 'blue' },
  { label: 'Conversion Rate', value: '3.42%', delta: '↑ 8.7% vs Apr 1 - Apr 30', tone: 'purple' },
  { label: 'Avg. Order Value', value: '$1,245', delta: '↓ 1.32% vs Apr 1 - Apr 30', tone: 'green' },
  { label: 'Churn Rate', value: '3.21%', delta: '↓ 0.53% vs Apr 1 - Apr 30', tone: 'red' },
]

const chartLabels = ['May 1', 'May 6', 'May 11', 'May 16', 'May 21', 'May 26', 'May 31']
const revenueValues = [220, 190, 205, 230, 290, 295, 245, 195, 190, 165, 175, 225, 235, 185, 170, 175, 230, 240, 225, 205, 185, 195, 215, 230, 240, 235, 205, 215, 360, 325, 295]
const expectedRevenueValues = [215, 205, 200, 210, 225, 235, 225, 215, 205, 200, 205, 215, 225, 215, 205, 200, 210, 220, 225, 220, 215, 215, 225, 235, 245, 240, 230, 235, 245, 250, 255]
const channelRows = [
  { name: 'Website', value: 42, color: '#f59e0b' },
  { name: 'Mobile', value: 27, color: '#fbbf24' },
  { name: 'Partner', value: 15, color: '#fcd34d' },
  { name: 'Retail', value: 9, color: '#f59e0b' },
  { name: 'Wholesale', value: 5, color: '#b45309' },
]

const segments = [
  { label: 'New Users', value: 36.4, color: '#3b82f6' },
  { label: 'Returning', value: 28.1, color: '#f97316' },
  { label: 'Enterprise', value: 17.7, color: '#a78bfa' },
  { label: 'Partners', value: 11.3, color: '#14b8a6' },
  { label: 'Other', value: 6.5, color: '#eab308' },
]
const dateRangeStart = ref('2025-05-01')
const dateRangeEnd = ref('2025-05-31')
const selectedSegments = ref(segments.map(segment => segment.label))
const datePickerOpen = ref(false)
const filtersOpen = ref(false)
const visibleSegments = computed(() => segments.filter(segment => selectedSegments.value.includes(segment.label)))
const userCount = computed(() => Math.round(1_234_333 * visibleSegments.value.reduce((total, segment) => total + segment.value, 0) / 100))
const formattedDateRange = computed(() => {
  if (!dateRangeStart.value || !dateRangeEnd.value) return 'Select date range'
  const start = new Date(`${dateRangeStart.value}T00:00:00`)
  const end = new Date(`${dateRangeEnd.value}T00:00:00`)
  const monthDay = new Intl.DateTimeFormat('en', { month: 'short', day: 'numeric' })
  const year = new Intl.DateTimeFormat('en', { year: 'numeric' })
  return `${monthDay.format(start)} – ${monthDay.format(end)}, ${year.format(end)}`
})

const anomalies = [
  { metric: 'Revenue', date: 'May 29, 2025', impact: 'High', confidence: '92%', severity: 'high' },
  { metric: 'Accounts', date: 'May 24, 2025', impact: 'Medium', confidence: '86%', severity: 'medium' },
  { metric: 'Traffic', date: 'May 18, 2025', impact: 'Medium', confidence: '76%', severity: 'medium' },
  { metric: 'Support', date: 'May 12, 2025', impact: 'Low', confidence: '65%', severity: 'low' },
  { metric: 'Marketing', date: 'May 7, 2025', impact: 'Low', confidence: '60%', severity: 'low' },
]

const chartPlot = { left: 42, top: 12, width: 650, height: 204 }
function revenuePoint(value: number, index: number) {
  const x = chartPlot.left + index / (revenueValues.length - 1) * chartPlot.width
  const y = chartPlot.top + chartPlot.height * (1 - value / 400)
  return { x, y }
}
const revenueLinePath = computed(() => revenueValues.map((value, index) => {
  const point = revenuePoint(value, index)
  return `${index === 0 ? 'M' : 'L'}${point.x.toFixed(1)},${point.y.toFixed(1)}`
}).join(' '))
const expectedLinePath = computed(() => expectedRevenueValues.map((value, index) => {
  const point = revenuePoint(value, index)
  return `${index === 0 ? 'M' : 'L'}${point.x.toFixed(1)},${point.y.toFixed(1)}`
}).join(' '))
const revenueAreaPath = computed(() => {
  const points = revenueValues.map((value, index) => revenuePoint(value, index))
  const lastPoint = points[points.length - 1]
  return `${revenueLinePath.value} L${lastPoint.x.toFixed(1)},${(chartPlot.top + chartPlot.height).toFixed(1)} L${points[0].x.toFixed(1)},${(chartPlot.top + chartPlot.height).toFixed(1)} Z`
})
const anomalyPoint = computed(() => revenuePoint(revenueValues[28], 28))
const anomalyX = computed(() => anomalyPoint.value.x)

const contributingFactors = [
  { label: 'New Users (Organic Search)', value: '+67%', width: '67%', icon: 'user' },
  { label: 'Avg. Order Value Increase', value: '+23%', width: '23%', icon: 'cart' },
  { label: 'Marketing Campaign: Spring Sale', value: '+18%', width: '18%', icon: 'campaign' },
]

const donutSegments = computed(() => {
  let cumulative = 0
  const visibleTotal = visibleSegments.value.reduce((total, segment) => total + segment.value, 0)
  if (visibleTotal === 0) return '#253247 0% 100%'
  return visibleSegments.value.map(segment => {
    const start = cumulative
    cumulative += segment.value / visibleTotal * 100
    const end = cumulative
    return `${segment.color} ${start}% ${end}%`
  }).join(', ')
})
</script>

<template>
  <main class="shell page-dashboard">
    <div class="dashboard-shell">
      <div class="dashboard-toolbar">
        <div class="toolbar-popover-wrap">
          <button
            class="dashboard-toolbar-button date-range-button"
            type="button"
            :aria-expanded="datePickerOpen"
            @click="datePickerOpen = !datePickerOpen; filtersOpen = false"
          >
            <svg viewBox="0 0 24 24" fill="none" aria-hidden="true">
              <rect x="3.5" y="5" width="17" height="15.5" rx="2" stroke="currentColor" stroke-width="1.6" />
              <path d="M7.5 3.5v3M16.5 3.5v3M3.5 9.5h17" stroke="currentColor" stroke-width="1.6" stroke-linecap="round" />
            </svg>
            <span>{{ formattedDateRange }}</span>
            <svg class="toolbar-chevron" viewBox="0 0 24 24" fill="none" aria-hidden="true"><path d="m7 10 5 5 5-5" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round" /></svg>
          </button>
          <div v-if="datePickerOpen" class="dashboard-popover date-range-popover">
            <label>Start date<input v-model="dateRangeStart" type="date" :max="dateRangeEnd || undefined" /></label>
            <label>End date<input v-model="dateRangeEnd" type="date" :min="dateRangeStart || undefined" /></label>
            <button class="toolbar-done-button" type="button" @click="datePickerOpen = false">Done</button>
          </div>
        </div>
        <div class="toolbar-popover-wrap">
          <button
            class="dashboard-toolbar-button filters-button"
            type="button"
            :aria-expanded="filtersOpen"
            @click="filtersOpen = !filtersOpen; datePickerOpen = false"
          >
            <svg viewBox="0 0 24 24" fill="none" aria-hidden="true"><path d="M4 6h16M7 12h10m-7 6h4" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" /></svg>
            <span>Filters</span>
            <svg class="toolbar-chevron" viewBox="0 0 24 24" fill="none" aria-hidden="true"><path d="m7 10 5 5 5-5" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round" /></svg>
          </button>
          <div v-if="filtersOpen" class="dashboard-popover filters-popover">
            <div class="filter-popover-heading">
              <strong>User segments</strong>
              <button type="button" @click="selectedSegments = segments.map(segment => segment.label)">Reset</button>
            </div>
            <label v-for="segment in segments" :key="segment.label" class="segment-filter-option">
              <input v-model="selectedSegments" type="checkbox" :value="segment.label" />
              <span class="legend-swatch" :style="{ background: segment.color }" />
              <span>{{ segment.label }}</span>
            </label>
          </div>
        </div>
      </div>
      <div class="stats-grid">
        <article v-for="card in topCards" :key="card.label" class="metric-card" :class="`metric-${card.tone}`">
          <div class="metric-header">
            <span class="metric-label">{{ card.label }}</span>
            <span class="metric-icon" aria-hidden="true">
              <svg viewBox="0 0 24 24" fill="none">
                <path d="M5 13.5 9.5 9l3 3L19 5.5" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" />
                <path d="M15 5.5h4v4" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" />
              </svg>
            </span>
          </div>
          <div class="metric-value">{{ card.value }}</div>
          <div class="metric-delta">{{ card.delta }}</div>
        </article>
      </div>

      <section class="hero-analytics">
        <div class="chart-panel">
          <div class="panel-header">
            <div class="chart-title-wrap">
              <span class="panel-title">Revenue Over Time</span>
              <span class="chart-info" role="img" aria-label="Revenue over time for May">i</span>
            </div>
            <div class="chart-controls">
              <select v-model="chartPeriod" class="chart-period" aria-label="Chart time interval">
                <option>Daily</option>
                <option>Weekly</option>
                <option>Monthly</option>
              </select>
              <div class="chart-mode-controls" aria-label="Chart type">
                <button type="button" :class="{ active: chartMode === 'line' }" aria-label="Line chart" @click="chartMode = 'line'">
                  <svg viewBox="0 0 24 24" fill="none" aria-hidden="true"><path d="M4 18.5h16M5.5 15l4-4 3 2.5 6-7" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round" /></svg>
                </button>
                <button type="button" :class="{ active: chartMode === 'area' }" aria-label="Area chart" @click="chartMode = 'area'">
                  <svg viewBox="0 0 24 24" fill="none" aria-hidden="true"><path d="M4 18.5h16M5 16l4-4 3 2.5 6-7v11H5Z" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" /></svg>
                </button>
                <button type="button" :class="{ active: chartMode === 'expected' }" aria-label="Compare with expected revenue" @click="chartMode = 'expected'">
                  <svg viewBox="0 0 24 24" fill="none" aria-hidden="true"><path d="M4 18.5h16M5 15l4-3 3 2 6-5" stroke="currentColor" stroke-width="1.6" stroke-linecap="round" stroke-dasharray="3 2" /></svg>
                </button>
              </div>
              <button class="chart-more-button" type="button" aria-label="More chart options" title="More chart options">⋮</button>
            </div>
          </div>

          <div class="chart-legend" aria-hidden="true">
            <span><i class="legend-dot revenue-dot" />Revenue</span>
            <span><i class="legend-dash" />Revenue (Expected)</span>
          </div>
          <div class="chart-surface" :class="`chart-mode-${chartMode}`" role="img" aria-label="Revenue over time chart">
            <div class="chart-y-labels" aria-hidden="true">
              <span>$400K</span><span>$300K</span><span>$200K</span><span>$100K</span><span>$0</span>
            </div>
            <svg class="revenue-plot" viewBox="0 0 710 240" preserveAspectRatio="none" aria-hidden="true">
              <defs>
                <linearGradient id="revenue-area-fill" x1="0" x2="0" y1="0" y2="1">
                  <stop offset="0%" stop-color="#f97316" stop-opacity=".2" />
                  <stop offset="100%" stop-color="#f97316" stop-opacity=".01" />
                </linearGradient>
              </defs>
              <g class="plot-grid">
                <line v-for="value in [0, 100, 200, 300, 400]" :key="value" x1="42" :y1="12 + 204 * (1 - value / 400)" x2="692" :y2="12 + 204 * (1 - value / 400)" />
              </g>
              <rect :x="anomalyX - 18" y="12" width="36" height="204" fill="#f97316" opacity=".09" />
              <path :d="revenueAreaPath" fill="url(#revenue-area-fill)" />
              <path :d="expectedLinePath" class="expected-revenue-line" />
              <path :d="revenueLinePath" class="actual-revenue-line" />
              <circle
                v-for="(value, index) in revenueValues"
                :key="index"
                :cx="revenuePoint(value, index).x"
                :cy="revenuePoint(value, index).y"
                :r="index === 28 ? 5.2 : 2.3"
                :class="{ 'anomaly-point': index === 28 }"
              />
            </svg>
            <div class="chart-labels">
              <span v-for="label in chartLabels" :key="label">{{ label }}</span>
            </div>
            <aside class="chart-anomaly-callout">
              <div class="callout-title"><span class="revenue-dot" />Anomaly Detected</div>
              <time>May 29, 2025</time>
              <p>Revenue was 46% higher than expected, driving $142K additional revenue.</p>
              <button type="button" class="callout-action" @click="emit('openVisualization')">View Anomaly</button>
              <div class="confidence-heading"><span>AI Confidence</span><strong>92%</strong></div>
              <div class="confidence-track"><span /></div>
            </aside>
          </div>
        </div>

        <aside class="insight-panel">
          <div class="insight-header">
            <span class="insight-title"><span class="insight-sparkle" aria-hidden="true">✦</span> AI Insight</span>
            <span class="impact-tag">High Impact</span>
          </div>
          <h3>Unusual spike in revenue detected on May 29.</h3>
          <p>This is due to a surge in New Users from Organic Search and a 23% increase in Avg. Order Value.</p>

          <h4 class="factor-heading">Top Contributing Factors</h4>
          <div class="factor-list">
            <div v-for="factor in contributingFactors" :key="factor.label" class="factor">
              <div class="factor-row">
                <span class="factor-icon" aria-hidden="true">
                  <svg v-if="factor.icon === 'user'" viewBox="0 0 24 24" fill="none"><circle cx="12" cy="8" r="3" stroke="currentColor" stroke-width="1.5" /><path d="M5.5 20a6.5 6.5 0 0 1 13 0" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" /></svg>
                  <svg v-else-if="factor.icon === 'cart'" viewBox="0 0 24 24" fill="none"><path d="M3 4h2l2.2 10.1a2 2 0 0 0 2 1.6h7.9a2 2 0 0 0 1.9-1.4L21 8H6" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" /><circle cx="10" cy="19" r="1" fill="currentColor" /><circle cx="18" cy="19" r="1" fill="currentColor" /></svg>
                  <svg v-else viewBox="0 0 24 24" fill="none"><path d="M4 10V5h5l9 9-5 5-9-9Zm3-2h.01" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" /></svg>
                </span>
                <span class="factor-name">{{ factor.label }}</span>
                <strong class="factor-value">{{ factor.value }}</strong>
              </div>
              <div class="factor-progress"><span :style="{ width: factor.width }" /></div>
            </div>
          </div>

          <button type="button" class="full-action" @click="emit('openVisualization')">View Full Analysis</button>
        </aside>
      </section>

      <section class="bottom-grid">
        <article class="mini-panel">
          <div class="mini-panel-header">
            <span>Revenue by Channel</span>
          </div>
          <div class="channel-list">
            <div v-for="row in channelRows" :key="row.name" class="channel-row">
              <div class="channel-label">{{ row.name }}</div>
              <div class="channel-bar">
                <span :style="{ width: `${row.value}%`, background: row.color }" />
              </div>
              <div class="channel-value">{{ row.value }}%</div>
            </div>
          </div>
        </article>

        <article class="mini-panel">
          <div class="mini-panel-header">
            <span>Users by Segment</span>
            <span class="chart-info" role="img" aria-label="User distribution by segment">i</span>
          </div>
          <div class="donut-wrap">
            <div
              class="donut-chart"
              role="img"
              :aria-label="`Users by segment: ${visibleSegments.map(item => `${item.label} ${item.value}%`).join(', ')}`"
              :style="{ background: `conic-gradient(${donutSegments})` }"
            >
              <div class="donut-inner">
                <strong>{{ new Intl.NumberFormat('en', { notation: 'compact', maximumFractionDigits: 2 }).format(userCount) }}</strong>
                <span>USERS</span>
              </div>
            </div>
            <div class="legend-list">
              <div v-for="item in visibleSegments" :key="item.label" class="legend-item">
                <span class="legend-swatch" :style="{ background: item.color }" />
                <span>{{ item.label }}</span>
                <strong>{{ item.value }}%</strong>
              </div>
            </div>
          </div>
        </article>

        <article class="mini-panel">
          <div class="mini-panel-header">
            <span>Recent Anomalies</span>
            <button type="button" class="text-link">View All</button>
          </div>
          <div class="anomaly-table">
            <div class="anomaly-head">
              <span>Metric</span>
              <span>Date</span>
              <span>Impact</span>
              <span>AI Confidence</span>
            </div>
            <div v-for="(item, index) in anomalies" :key="`${item.metric}-${index}`" class="anomaly-row">
              <span>{{ item.metric }}</span>
              <span>{{ item.date }}</span>
              <span :class="`impact-${item.severity}`">{{ item.impact }}</span>
              <span>{{ item.confidence }}</span>
            </div>
          </div>
        </article>
      </section>
    </div>
  </main>
</template>
