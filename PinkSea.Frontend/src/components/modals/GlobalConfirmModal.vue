<script setup lang="ts">
import Modal from '@/components/Modal.vue'
import { useTemplateRef, watch } from 'vue'
import { useConfirm } from '@/api/modals/confirm'

const modal = useTemplateRef('modal')
const { modalTextKey, modalTitleKey, isOpen, resolveConfirm } = useConfirm()

const onDismiss = () => {
    resolveConfirm(false)
    isOpen.value = false
}

const onYes = () => {
    resolveConfirm(true)
    modal.value?.dismiss()
}

const onNo = () => {
    resolveConfirm(false)
    modal.value?.dismiss()
}

watch(isOpen, () => {
    console.log(isOpen.value)
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
            <button v-on:click="onYes">{{ $t('modal.yes') }}</button>
            <button v-on:click="onNo">{{ $t('modal.no') }}</button>
        </template>
    </Modal>
</template>

<style shared></style>
