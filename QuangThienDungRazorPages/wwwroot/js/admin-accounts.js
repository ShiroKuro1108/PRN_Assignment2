// Admin Accounts Management JavaScript

// Create Account Form Handler
document.getElementById('createAccountForm').addEventListener('submit', async function(e) {
    e.preventDefault();
    
    const formData = {
        name: document.getElementById('createAccountName').value,
        email: document.getElementById('createAccountEmail').value,
        password: document.getElementById('createAccountPassword').value,
        role: parseInt(document.getElementById('createAccountRole').value)
    };

    try {
        const response = await fetch('/Admin/Accounts?handler=Create', {
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
            document.getElementById('createAccountForm').reset();
            bootstrap.Modal.getInstance(document.getElementById('createAccountModal')).hide();
            setTimeout(() => location.reload(), 1000);
        } else {
            showAlert('danger', result.message);
        }
    } catch (error) {
        showAlert('danger', 'An error occurred while creating the account.');
    }
});

// Edit Account Form Handler
document.getElementById('editAccountForm').addEventListener('submit', async function(e) {
    e.preventDefault();
    
    const formData = {
        id: parseInt(document.getElementById('editAccountId').value),
        name: document.getElementById('editAccountName').value,
        email: document.getElementById('editAccountEmail').value,
        password: document.getElementById('editAccountPassword').value,
        role: parseInt(document.getElementById('editAccountRole').value)
    };

    try {
        const response = await fetch('/Admin/Accounts?handler=Update', {
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
            bootstrap.Modal.getInstance(document.getElementById('editAccountModal')).hide();
            setTimeout(() => location.reload(), 1000);
        } else {
            showAlert('danger', result.message);
        }
    } catch (error) {
        showAlert('danger', 'An error occurred while updating the account.');
    }
});

// Edit Account Function
async function editAccount(accountId) {
    try {
        const response = await fetch(`/Admin/Accounts?handler=Account&id=${accountId}`);
        const result = await response.json();
        
        if (result.success) {
            const account = result.account;
            document.getElementById('editAccountId').value = account.id;
            document.getElementById('editAccountName').value = account.name;
            document.getElementById('editAccountEmail').value = account.email;
            document.getElementById('editAccountPassword').value = '';
            document.getElementById('editAccountRole').value = account.role;
            
            new bootstrap.Modal(document.getElementById('editAccountModal')).show();
        } else {
            showAlert('danger', result.message);
        }
    } catch (error) {
        showAlert('danger', 'An error occurred while loading the account.');
    }
}

// Delete Account Function
async function deleteAccount(accountId, accountName) {
    if (!confirm(`Are you sure you want to delete the account "${accountName}"? This action cannot be undone.`)) {
        return;
    }

    try {
        const response = await fetch('/Admin/Accounts?handler=Delete', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getAntiForgeryToken()
            },
            body: JSON.stringify({ id: accountId })
        });

        const result = await response.json();
        
        if (result.success) {
            showAlert('success', result.message);
            setTimeout(() => location.reload(), 1000);
        } else {
            showAlert('danger', result.message);
        }
    } catch (error) {
        showAlert('danger', 'An error occurred while deleting the account.');
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
