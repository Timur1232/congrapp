import { createContext, useContext, useEffect, useState, ReactNode } from 'react';
import authApi from '../AuthApi';
import { User } from '../AuthApi';

interface AuthContextProps {
    user: User | null;
    login: (email: string, password: string) => Promise<void>;
    register: (email: string, password: string, passwordConfirmation: string) => Promise<void>;
    logout: () => void;
}

const AuthContext = createContext<AuthContextProps | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
    const [user, setUser] = useState<User | null>(null);

    useEffect(() => {
        const loadUser = async () => {
            try {
                if (authApi.isAuthenticated()) {
                    const userData = await authApi.me();
                    setUser(userData);
                }
            } catch (error) {
                console.error('Session check failed', error);
            }
        };

        loadUser();
    }, []);

    const login = async (email: string, password: string) => {
        const { token } = await authApi.login(email, password);
        localStorage.setItem('token', token);
        const userData = await authApi.me();
        setUser(userData);
    };

    const register = async (email: string, password: string, passwordConfirmation: string) => {
        const { token } = await authApi.register(email, password, passwordConfirmation);
        localStorage.setItem('token', token);
        const userData = await authApi.me();
        setUser(userData);
    };

    const logout = () => {
        authApi.logout();
        setUser(null);
    };

    return (
        <AuthContext.Provider value={{ user, login, register, logout }}>
            {children}
        </AuthContext.Provider>
    );
}

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (context === undefined) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
};