<script setup lang="ts">

</script>

<template>
  <div class="bar">
    <span><RouterLink to="/">PinkSea</RouterLink></span>
    &gt;
    <span class="bar-current">recent</span>
  </div>
</template>

<style scoped>
  .bar {
    margin: 10px;
  }
  .bar * {
    text-decoration: none;
    color: black;
  }
  .bar *:hover {
    text-decoration: underline;
    cursor: pointer;
  }
  .bar-current {
    font-weight: bold;
  }
</style>
