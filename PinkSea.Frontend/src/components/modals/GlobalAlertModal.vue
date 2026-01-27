<script setup lang="ts">
import Modal from '@/components/Modal.vue'
import { useAlert } from '@/api/modals/alert'
import { useTemplateRef, watch } from 'vue'

const modal = useTemplateRef('modal')
const { modalTextKey, modalTitleKey, isOpen } = useAlert()

const onDismiss = () => {
    isOpen.value = false
}

watch(isOpen, () => {
    if (isOpen.value) {
        modal.value?.show()
    } else {
        modal.value?.dismiss()
    }
})
</script>

<template>
    <Modal ref="modal" v-on:dismissed="onDismiss">
        <template #header>
            <div>{{ $t(modalTitleKey) }}</div>
        </template>
        <template #body>
            <p>{{ $t(modalTextKey) }}</p>
            <button v-on:click="modal?.dismiss()">{{ $t('modal.ok') }}</button>
        </template>
    </Modal>
</template>

<style shared></style>
