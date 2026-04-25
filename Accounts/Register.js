function validateAndDisable(btn) {
    if (validateForm()) {
        btn.disabled = true;
        btn.value = "Creating Account...";
        return true;
    }
    return false;
}