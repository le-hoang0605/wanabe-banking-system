import api from './api.js';

const loginForm = document.getElementById('loginForm');
const errorBox = document.getElementById('errorBox');

loginForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    
    // Reset lỗi cũ
    errorBox.classList.add('hidden');
    errorBox.textContent = '';
    
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;

    try {
        const response = await api.post('/api/Auth/login', {
            email: email,
            password: password
        });

        const { token } = response.data;
        localStorage.setItem('jwt_token', token);
        
        window.location.href = '../pages/dashboard.html';
        
    } catch (error) {
        console.error('Login failed:', error);
        
        // Lấy thông báo lỗi từ server hoặc mặc định
        const message = error.response?.data?.message || 'Invalid email or password.';
        
        // Hiển thị lỗi lên UI
        errorBox.textContent = message;
        errorBox.classList.remove('hidden');
    }
});