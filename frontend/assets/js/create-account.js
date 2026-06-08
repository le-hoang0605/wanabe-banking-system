import api from './api.js';

const form = document.getElementById('createAccountForm');
const submitBtn = form.querySelector('button[type="submit"]');


const successModal = document.getElementById('successModal');
const newAccountNumberDisplay = document.getElementById('newAccountNumber');
const btnGoToProfile = document.getElementById('btnGoToProfile');

let cachedPartyId = null;

async function initialize() {
    try {
        submitBtn.disabled = true;
        submitBtn.innerText = "Loading profile...";
        
        const profileRes = await api.get('/api/Auth/profile');
        
        cachedPartyId = profileRes.data.userId;
        
        if (!cachedPartyId) {
            alert("Error: userId not found in profile data.");
            submitBtn.innerText = "Error ID";
            return;
        }
        
        submitBtn.disabled = false;
        submitBtn.innerText = "Create Account";
    } catch (err) {
        console.error("Failed to load profile:", err);
        submitBtn.innerText = "Error Loading Profile";
        alert("Session expired. Please login again.");
        window.location.href = 'login.html';
    }
}

initialize();

form.addEventListener('submit', async (e) => {
    e.preventDefault();
    
    if (!cachedPartyId) {
        alert("Please wait for profile data to load or re-login.");
        return;
    }

    try {
        submitBtn.disabled = true;
        submitBtn.innerText = "Creating...";

        const payload = {
            partyId: cachedPartyId,
            initialBalance: parseFloat(document.getElementById('initialBalance').value),
            currency: 'VND', 
            isManager: document.getElementById('isManager').value === 'true'
        };

        const response = await api.post('/api/accounts/create', payload);
        const accountNumber = response.data.accountNumber;

        newAccountNumberDisplay.innerText = accountNumber;
        successModal.classList.remove('hidden');
        
    } catch (err) {
        console.error(err);
        submitBtn.disabled = false;
        submitBtn.innerText = "Create Account";
        
        const errorMessage = err.response?.data?.title || err.response?.data?.message || "Failed to create account.";
        alert("Error: " + errorMessage);
    }
});

btnGoToProfile.addEventListener('click', () => {
    // Chuyển về trang profile của bạn (chú ý sửa đường dẫn cho đúng thư mục của bạn nhé)
    window.location.href = 'profile.html'; 
});