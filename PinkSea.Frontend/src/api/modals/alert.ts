import { ref } from 'vue'

const isOpen = ref(false)
const modalTitleKey = ref<string>("")
const modalTextKey = ref<string>("")

export function useAlert() {
    function showAlert(title: string, body?: string) {
        isOpen.value = true
        modalTitleKey.value = title
        modalTextKey.value = body ?? ""
    }

    function showErrorAlert(body: string) {
        showAlert("modal.generic_error", body)
    }

    function showWarningAlert(body: string) {
        showAlert("modal.generic_warning", body)
    }

    function showInformationalAlert(body: string) {
        showAlert("modal.generic_informational", body)
    }

    return { isOpen, modalTitleKey, modalTextKey, showAlert, showErrorAlert, showWarningAlert, showInformationalAlert }
}
