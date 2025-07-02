// Staff Categories Management JavaScript

// Create Category Form Handler
document.getElementById('createCategoryForm').addEventListener('submit', async function(e) {
    e.preventDefault();
    
    const formData = {
        name: document.getElementById('createCategoryName').value,
        description: document.getElementById('createCategoryDescription').value,
        parentCategoryId: document.getElementById('createParentCategory').value || null,
        isActive: document.getElementById('createIsActive').checked
    };

    try {
        const response = await fetch('/Staff/Categories?handler=Create', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getAntiForgeryToken()
            },
            body: JSON.stringify(formData)
        });

        const result = await response.json();
        
        if (result.success) {
            showAlert('success', result.message);
            document.getElementById('createCategoryForm').reset();
            bootstrap.Modal.getInstance(document.getElementById('createCategoryModal')).hide();
            setTimeout(() => location.reload(), 1000);
        } else {
            showAlert('danger', result.message);
        }
    } catch (error) {
        showAlert('danger', 'An error occurred while creating the category.');
    }
});

// Edit Category Form Handler
document.getElementById('editCategoryForm').addEventListener('submit', async function(e) {
    e.preventDefault();
    
    const formData = {
        id: parseInt(document.getElementById('editCategoryId').value),
        name: document.getElementById('editCategoryName').value,
        description: document.getElementById('editCategoryDescription').value,
        parentCategoryId: document.getElementById('editParentCategory').value || null,
        isActive: document.getElementById('editIsActive').checked
    };

    try {
        const response = await fetch('/Staff/Categories?handler=Update', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getAntiForgeryToken()
            },
            body: JSON.stringify(formData)
        });

        const result = await response.json();
        
        if (result.success) {
            showAlert('success', result.message);
            bootstrap.Modal.getInstance(document.getElementById('editCategoryModal')).hide();
            setTimeout(() => location.reload(), 1000);
        } else {
            showAlert('danger', result.message);
        }
    } catch (error) {
        showAlert('danger', 'An error occurred while updating the category.');
    }
});

// Edit Category Function
async function editCategory(categoryId) {
    try {
        const response = await fetch(`/Staff/Categories?handler=Category&id=${categoryId}`);
        const result = await response.json();
        
        if (result.success) {
            const category = result.category;
            document.getElementById('editCategoryId').value = category.id;
            document.getElementById('editCategoryName').value = category.name;
            document.getElementById('editCategoryDescription').value = category.description;
            document.getElementById('editParentCategory').value = category.parentCategoryId || '';
            document.getElementById('editIsActive').checked = category.isActive;
            
            new bootstrap.Modal(document.getElementById('editCategoryModal')).show();
        } else {
            showAlert('danger', result.message);
        }
    } catch (error) {
        showAlert('danger', 'An error occurred while loading the category.');
    }
}

// Delete Category Function
async function deleteCategory(categoryId, categoryName) {
    if (!confirm(`Are you sure you want to delete the category "${categoryName}"? This action cannot be undone.`)) {
        return;
    }

    try {
        const response = await fetch('/Staff/Categories?handler=Delete', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getAntiForgeryToken()
            },
            body: JSON.stringify({ id: categoryId })
        });

        const result = await response.json();
        
        if (result.success) {
            showAlert('success', result.message);
            setTimeout(() => location.reload(), 1000);
        } else {
            showAlert('danger', result.message);
        }
    } catch (error) {
        showAlert('danger', 'An error occurred while deleting the category.');
    }
}

// Utility Functions
function getAntiForgeryToken() {
    return document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
}

function showAlert(type, message) {
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type} alert-dismissible fade show`;
    alertDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    
    const container = document.querySelector('.container-fluid');
    container.insertBefore(alertDiv, container.firstChild);
    
    // Auto-dismiss after 5 seconds
    setTimeout(() => {
        if (alertDiv.parentNode) {
            alertDiv.remove();
        }
    }, 5000);
}
