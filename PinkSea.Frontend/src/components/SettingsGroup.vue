<script setup lang="ts">

const props = defineProps<{
    intlKey: string
}>();

</script>

<template>
    <fieldset>
        <legend>{{ $t(props.intlKey) }}</legend>
        <slot>

        </slot>
    </fieldset>
</template>

<style scoped>
legend {
    font-weight: bold;
}

legend:before {
    content: ">>";
    color: #FFB6C1;
    letter-spacing: -4px;
    margin-left: -6.5px;
    margin-right: 4px;
}

fieldset {
    border: 2px solid #FFB6C1;
}
</style>