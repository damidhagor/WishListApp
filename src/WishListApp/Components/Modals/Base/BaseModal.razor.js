function getModal(modalId) {
    const modalIdSelector = `#${modalId}`

    return [
        document.getElementById(modalId),
        bootstrap.Modal.getOrCreateInstance(modalIdSelector)
    ];
}

export function initModal(modalId, modalReference) {
    const [modalElement, modal] = getModal(modalId);

    modalElement.addEventListener(
        'show.bs.modal',
        event => {
            modalReference.invokeMethodAsync('OnBootstrapModalShow');
        });

    modalElement.addEventListener(
        'shown.bs.modal',
        event => {
            modalReference.invokeMethodAsync('OnBootstrapModalShown');
        });

    modalElement.addEventListener(
        'hide.bs.modal',
        event => {
            modalReference.invokeMethodAsync('OnBootstrapModalHide');
        });

    modalElement.addEventListener(
        'hidden.bs.modal',
        event => {
            modalReference.invokeMethodAsync('OnBootstrapModalHidden');
        });

    modalElement.addEventListener(
        'hidePrevented.bs.modal',
        event => {
            modalReference.invokeMethodAsync('OnBootstrapModalHidePrevented');
        });
}

export function showModal(modalId) {
    const [modalElement, modal] = getModal(modalId);

    modal.show();
}

export function hideModal(modalId) {
    const [modalElement, modal] = getModal(modalId);

    modal.hide();
}