<script setup lang="ts">
  const props = defineProps<{
    image: string,
    i18nKey: string
  }>()
</script>

<template>
  <div class="oekaki-card">
    <img :src="props.image" />
    <div class="oekaki-meta">
      {{ $t(props.i18nKey) }}
    </div>
  </div>
</template>

<style scoped>
.oekaki-card {
  display: inline-block;
  border: 2px solid #FFB6C1;
  width: calc(100% - 20px);
  box-sizing: border-box;
  margin: 10px;
  z-index: 5;
  position: relative;
  text-align: center;
  background-size: 8px 8px;
  background-image: repeating-linear-gradient(45deg, #FFB6C1 0, #FFB6C1 0.8px, #FFFFFF 0, #FFFFFF 50%);
}

.oekaki-meta {
  font-size: small;
  padding: 10px;
  color: #2f4858;
  border-top: 2px dashed #FFB6C1;
  border-left: 0.525em solid #FFB6C1;
  background-color: white;
}

img {
  max-width: 80%;
  background-color: white;
  padding: 20px;
}
</style>
