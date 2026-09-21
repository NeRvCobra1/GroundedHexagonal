window.hexVisualizer = {
    scrollStepIntoView: function (index) {
        const viewport = document.getElementById("flowViewport");
        const node = document.querySelector(`[data-flow-step="${index}"]`);

        if (!viewport || !node) {
            return;
        }

        const viewportRect = viewport.getBoundingClientRect();
        const nodeRect = node.getBoundingClientRect();
        const viewportCenter = viewportRect.left + (viewportRect.width / 2);
        const nodeCenter = nodeRect.left + (nodeRect.width / 2);
        const delta = nodeCenter - viewportCenter;

        viewport.scrollBy({
            left: delta,
            behavior: "smooth"
        });
    },

    enableFlowPan: function (viewportId) {
        const viewport = document.getElementById(viewportId);

        if (!viewport || viewport.dataset.panEnabled === "true") {
            return;
        }

        viewport.dataset.panEnabled = "true";

        const dragThreshold = 5;
        let tracking = false;
        let dragging = false;
        let suppressNextClick = false;
        let pointerId = null;
        let startX = 0;
        let startY = 0;
        let startScrollLeft = 0;

        const resetPointerState = () => {
            if (pointerId !== null && viewport.hasPointerCapture(pointerId)) {
                viewport.releasePointerCapture(pointerId);
            }

            tracking = false;
            dragging = false;
            pointerId = null;
            viewport.classList.remove("is-panning");
        };

        viewport.addEventListener("pointerdown", (event) => {
            if (event.button !== 0) {
                return;
            }

            // Keep explicit toolbar/form controls usable if any are added to the canvas later.
            // Runtime flow nodes are buttons on purpose and ARE draggable: a short press remains
            // a click, while movement beyond the threshold becomes a pan gesture.
            if (event.target.closest("a, input, select, textarea")) {
                return;
            }

            tracking = true;
            dragging = false;
            suppressNextClick = false;
            pointerId = event.pointerId;
            startX = event.clientX;
            startY = event.clientY;
            startScrollLeft = viewport.scrollLeft;

        });

        viewport.addEventListener("pointermove", (event) => {
            if (!tracking || event.pointerId !== pointerId) {
                return;
            }

            const deltaX = event.clientX - startX;
            const deltaY = event.clientY - startY;

            if (!dragging) {
                const distance = Math.hypot(deltaX, deltaY);

                if (distance < dragThreshold) {
                    return;
                }

                dragging = true;
                suppressNextClick = true;
                viewport.setPointerCapture(pointerId);
                viewport.classList.add("is-panning");
            }

            viewport.scrollLeft = startScrollLeft - deltaX;
            event.preventDefault();
        });

        viewport.addEventListener("pointerup", (event) => {
            if (!tracking || event.pointerId !== pointerId) {
                return;
            }

            // preventDefault only after a real drag. A simple pointer down/up continues through
            // to the Blazor button click and selects the runtime step normally.
            if (dragging) {
                event.preventDefault();
            }

            resetPointerState();
        });

        viewport.addEventListener("pointercancel", resetPointerState);
        viewport.addEventListener("lostpointercapture", () => {
            tracking = false;
            dragging = false;
            pointerId = null;
            viewport.classList.remove("is-panning");
        });

        // A browser still emits a click after pointerup on a button. If the pointer moved enough
        // to pan, intercept that synthetic click in capture phase so Blazor does not change step.
        viewport.addEventListener("click", (event) => {
            if (!suppressNextClick) {
                return;
            }

            suppressNextClick = false;
            event.preventDefault();
            event.stopPropagation();
        }, true);
    }
};
