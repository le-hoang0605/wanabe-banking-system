import api from './api.js';

const signupForm = document.getElementById('signupForm');

signupForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    
    const fullName = document.getElementById('fullName').value;
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;

    try {
        await api.post('/api/Auth/register', {
            fullName,
            email,
            password
        });

        alert('Registered Successfully!');
        window.location.href = '../pages/login.html';
        
    } catch (error) {
        console.error('Registration failed:', error);
        alert(error.response?.data?.message || 'Registration failed.');
    }
});