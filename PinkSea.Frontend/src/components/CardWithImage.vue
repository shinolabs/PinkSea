<script setup lang="ts">

const props = defineProps<{
  imageUrl: string
}>();

</script>

<template>
  <div class="card">
    <img :src="props.imageUrl">
    <div class="card-slot">
      <slot></slot>
    </div>
  </div>
</template>

<style scoped>
  .card {
    display: flex;
    align-items: center;
    border: 2px solid #FFB6C1;
    width: 100%;
    margin-top: 10px;
    border-left: 0.525em solid #FFB6C1;
    box-sizing: border-box;
    position: relative;
    padding: 5px;
  }

  .card > * {
    margin-right: 10px;
  }

  .card > img {
    max-width: 80px;
  }
</style>
