<script setup lang="ts">
  import { useBreadcrumbBarStore } from '@/api/breadcrumb/store'

  const store = useBreadcrumbBarStore();
</script>

<template>
  <div class="bar">
    <RouterLink to="/" class="bar-current">PinkSea</RouterLink>
    <span v-for="crumb of store.crumbs" v-bind:key="crumb">
      >
      <span>{{ crumb }}</span>
    </span>
  </div>
</template>

<style scoped>
.bar {
  margin: 0px 0px 0px 0px;
  padding: 10px 34px 0px 10px;
  background-color: #FFF5F6;
  border-bottom: 1px dashed #FFB6C1;
  padding-bottom: 8px;
}

.bar * {
  text-decoration: none;
  color: black;
}

.bar *:hover {
  text-decoration: underline;
  cursor: pointer;
}

.bar:before {
  content: "》》》";
  letter-spacing: -3px;
  margin-right: 8px;
  color: #FFB6C1;
  font-weight: bold;
}

.bar-current {
  font-weight: bold;
}
</style>
