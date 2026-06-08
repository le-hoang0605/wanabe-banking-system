const api = axios.create({
    baseURL: 'http://localhost:5296', 
    timeout: 10000,
});

api.interceptors.request.use((config) => {
    const token = sessionStorage.getItem('jwt_token') || localStorage.getItem('jwt_token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});


api.interceptors.response.use(
    response => response,
    error => {
        // if (error.response?.status === 401) {
        //     window.location.href = '../pages/login.html';
        // }
        return Promise.reject(error);
    }
);

export default api;