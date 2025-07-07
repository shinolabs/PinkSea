<script setup lang="ts">

const props = defineProps<{
    image: string
}>()

</script>

<template>
    <img :src="props.image" />
</template>

<style scoped></style>