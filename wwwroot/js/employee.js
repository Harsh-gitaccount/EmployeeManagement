let isEditMode = false;

// Load employees when page loads
document.addEventListener('DOMContentLoaded', function() {
    loadEmployees();
    setupEventListeners();
});

// Setup all event listeners
function setupEventListeners() {
    // Form submit
    document.getElementById('employeeForm').addEventListener('submit', function(e) {
        e.preventDefault();
        saveEmployee();
    });

    // Cancel button
    document.getElementById('cancelBtn').addEventListener('click', function() {
        resetForm();
    });

    // Image preview
    document.getElementById('profileImage').addEventListener('change', function(e) {
        previewImage(e);
    });

    // Remove image button
    document.getElementById('removeImageBtn').addEventListener('click', function() {
        removeImage();
    });
}

// Load all employees
function loadEmployees() {
    fetch('/Employee/GetAllEmployees')
        .then(response => response.json())
        .then(data => {
            displayEmployees(data);
        })
        .catch(error => {
            console.error('Error loading employees:', error);
            alert('Error loading employees!');
        });
}

// Display employees in table
function displayEmployees(employees) {
    const tbody = document.getElementById('employeeTableBody');
    tbody.innerHTML = '';

    if (employees.length === 0) {
        tbody.innerHTML = '<tr><td colspan="6" class="text-center">No employees found</td></tr>';
        return;
    }

    employees.forEach((employee, index) => {
        const row = document.createElement('tr');
        
        // Profile image or "No image"
        let profileHtml = '';
        if (employee.picture) {
            profileHtml = `<img src="${employee.picture}" alt="Profile" style="max-width: 50px; max-height: 50px; border-radius: 5px;" />`;
        } else {
            profileHtml = '<span class="text-muted">No image</span>';
        }

        row.innerHTML = `
    <td>${index + 1}</td>
    <td>${employee.name}</td>
    <td>${employee.email}</td>
    <td>${profileHtml}</td>
    <td>${employee.qualification}</td>
    <td>
        <button class="btn btn-sm btn-primary me-1" onclick="editEmployee(${employee.id})">
            <i class="bi bi-pencil"></i> Edit
        </button>
        <button class="btn btn-sm btn-danger" onclick="deleteEmployee(${employee.id}, '${employee.name}')">
            <i class="bi bi-trash"></i> Delete
        </button>
    </td>
`;

        
        tbody.appendChild(row);
    });
}

// Save employee (Add or Update)
function saveEmployee() {
    // Get qualification checkboxes
    const qualifications = [];
    if (document.getElementById('diploma').checked) qualifications.push('Diploma');
    if (document.getElementById('degree').checked) qualifications.push('Degree');
    if (document.getElementById('masterDegree').checked) qualifications.push('Master Degree');

    // Validation
    const name = document.getElementById('name').value.trim();
    const email = document.getElementById('email').value.trim();

    if (!name || !email) {
        alert('Name and Email are required!');
        return;
    }

    if (qualifications.length === 0) {
        alert('Please select at least one qualification!');
        return;
    }

    // Prepare form data
    const formData = new FormData();
    const employeeId = document.getElementById('employeeId').value;
    
    formData.append('ID', employeeId);
    formData.append('Name', name);
    formData.append('Email', email);
    formData.append('Qualification', qualifications.join(', '));

    // Add image if selected
    const fileInput = document.getElementById('profileImage');
    if (fileInput.files.length > 0) {
        formData.append('profileImage', fileInput.files[0]);
    }

    // Determine URL based on mode
    const url = isEditMode ? '/Employee/UpdateEmployee' : '/Employee/AddEmployee';

    // Submit form via AJAX
    fetch(url, {
        method: 'POST',
        body: formData
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            alert(data.message);
            resetForm();
            loadEmployees();
        } else {
            alert(data.message);
        }
    })
    .catch(error => {
        console.error('Error saving employee:', error);
        alert('Error saving employee!');
    });
}

// Edit employee
function editEmployee(id) {
    fetch('/Employee/GetEmployeeById?id=' + id)
        .then(response => response.json())
        .then(employee => {
            if (employee) {
                // Switch to edit mode
                isEditMode = true;
                document.getElementById('formTitle').textContent = 'EDIT EMPLOYEE';
                document.getElementById('submitBtn').textContent = 'UPDATE';

                // Fill form
                document.getElementById('employeeId').value = employee.id;
                document.getElementById('name').value = employee.name;
                document.getElementById('email').value = employee.email;

                // Set qualifications
                document.getElementById('diploma').checked = employee.qualification.includes('Diploma');
                document.getElementById('degree').checked = employee.qualification.includes('Degree');
                document.getElementById('masterDegree').checked = employee.qualification.includes('Master Degree');

                // Show existing image if available
                if (employee.picture) {
                    document.getElementById('previewImg').src = employee.picture;
                    document.getElementById('imagePreview').style.display = 'block';
                }

                // Scroll to form
                window.scrollTo({ top: 0, behavior: 'smooth' });
            }
        })
        .catch(error => {
            console.error('Error loading employee:', error);
            alert('Error loading employee data!');
        });
}

// Reset form
function resetForm() {
    isEditMode = false;
    document.getElementById('formTitle').textContent = 'ADD EMPLOYEE';
    document.getElementById('submitBtn').textContent = 'SAVE';
    document.getElementById('employeeForm').reset();
    document.getElementById('employeeId').value = '0';
    document.getElementById('imagePreview').style.display = 'none';
    document.getElementById('previewImg').src = '';
}

// Preview selected image
function previewImage(event) {
    const file = event.target.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = function(e) {
            document.getElementById('previewImg').src = e.target.result;
            document.getElementById('imagePreview').style.display = 'block';
        };
        reader.readAsDataURL(file);
    }
}

// Remove image
function removeImage() {
    document.getElementById('profileImage').value = '';
    document.getElementById('previewImg').src = '';
    document.getElementById('imagePreview').style.display = 'none';
}

// Delete employee
function deleteEmployee(id, name) {
    // Confirm before deleting
    if (!confirm(`Are you sure you want to delete employee "${name}"?`)) {
        return;
    }

    fetch('/Employee/DeleteEmployee', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
        },
        body: 'id=' + id
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            alert(data.message);
            loadEmployees();
            
            // If the deleted employee was being edited, reset form
            if (document.getElementById('employeeId').value == id) {
                resetForm();
            }
        } else {
            alert(data.message);
        }
    })
    .catch(error => {
        console.error('Error deleting employee:', error);
        alert('Error deleting employee!');
    });
}
