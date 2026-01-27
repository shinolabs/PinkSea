import { ref } from 'vue'

const isOpen = ref(false)
const modalTitleKey = ref<string>("")
const modalTextKey = ref<string>("")
let resolvePromise: ((value: boolean) => void) | null = null;

export function useConfirm() {
    function showConfirm(title: string, body?: string): Promise<boolean> {
        console.log("abcd")
        isOpen.value = true
        modalTitleKey.value = title
        modalTextKey.value = body ?? ""

        return new Promise<boolean>((resolve) => {
            resolvePromise = resolve
        })
    }

    function showWarningConfirm(body: string) {
        return showConfirm('modal.generic_warning', body)
    }

    function showQuestionConfirm(body: string) {
        return showConfirm('modal.generic_question', body)
    }

    function resolveConfirm(value: boolean) {
        if (!resolvePromise) {
            return
        }

        resolvePromise(value)
        resolvePromise = null
    }

    return { isOpen, modalTitleKey, modalTextKey, showConfirm, showQuestionConfirm, showWarningConfirm, resolveConfirm }
}
