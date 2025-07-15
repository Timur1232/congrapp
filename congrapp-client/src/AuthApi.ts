export interface BirthdayInfo {
    id: number;
    personName: string;
    birthdayDate: string;
    note?: string;
    hasImage: boolean;
}

export interface User{
    email: string;
    emailVerified: boolean;
}

export interface NotificationRecord {
    id: number;
    birthdayId: number;
    daysBefore: number;
}

class AuthApi {
    readonly baseUrl = process.env.REACT_APP_API_BASE_URL || 'http://localhost:8080';
    
    async login(email: string, password: string) {
        const res = await fetch(`${(this.baseUrl)}/api/auth/login`, {
            method: 'POST',
            body: JSON.stringify({email, password}),
            headers: {
                'Content-Type': 'application/json',
            }
        });
        if (!res.ok) {
            throw new Error('Login error');
        }
        return await res.json();
    }
    
    async register(email: string, password: string, passwordConfirmation: string) {
        const res = await fetch(`${(this.baseUrl)}/api/auth/register`, {
            method: 'POST',
            body: JSON.stringify({email, password, passwordConfirmation}),
            headers: {
                'Content-Type': 'application/json',
            }
        });
        if (!res.ok) {
            throw new Error('Register error');
        }
        return await res.json();
    }
    
    async me() {
        return await this.fetchAuthJson<User>(`/api/auth/me`, {
            method: 'GET'
        });
    }
    
    logout() {
        localStorage.removeItem('token');
    }
    
    getToken() {
        return localStorage.getItem('token');
    }
    
    async fetchAuth(url: string, request: RequestInit = {}): Promise<Response> {
        const headers = {
            "Authorization": `Bearer ${this.getToken()}`,
            ...(request?.headers || {})
        };
        const res = await fetch(`${(this.baseUrl)}${url}`, {...request, headers});
        if (!res.ok) {
            throw new Error('Request error');
        }
        return res;
    }
    
    isAuthenticated() {
        return !!localStorage.getItem('token');
    }

    async fetchAuthJson<T>(url: string, request: RequestInit = {}): Promise<T> {
        const res = await this.fetchAuth(`${url}`, {...request});
        return await res.json() as Promise<T>;
    }
}

export default new AuthApi();