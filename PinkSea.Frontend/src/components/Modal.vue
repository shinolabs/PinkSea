<script setup lang="ts">
import { useTemplateRef } from 'vue';

const dialog = useTemplateRef("dialog");
const show = () => {
    dialog.value?.showModal();
};

const dismiss = () => {
    dialog.value?.close();
};

defineExpose({
    show,
    dismiss
})

</script>

<template>
    <teleport to="body">
        <dialog ref="dialog">
            <strong>
                <slot name="header"></slot>
            </strong>
            <div id="dlg-content">
                <slot name="body"></slot>
            </div>
        </dialog>
    </teleport>
</template>

<style scoped>
dialog {
    text-align: center;
    padding: 0px;
    border: 4px solid #ffb6c1;
    box-shadow: -6px 6px rgba(38 59 72/0.4);
}

dialog button {
    display: inline-block;
    width: 45%;
}

dialog p {
    text-align: initial;
}

/* loc just-in-case */

#dlg-content {
    padding: 0px 16px 12px 16px;
    border-bottom: 6px double #ffb6c1;
}

dialog strong {
    display: block;
    letter-spacing: 1px;
    background: #ffb6c1;
    color: #263b48;
    padding: 0px 4px 4px 4px;
    box-shadow: 0px 2px #FFDADF;
}

::backdrop {
    background: url("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAIAAAACCAYAAABytg0kAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAZSURBVBhXY1Sz9tjHwMBwnQHImMrAwMAAACTCA37lqSVSAAAAAElFTkSuQmCC");
    background-repeat: repeat;
}
</style>