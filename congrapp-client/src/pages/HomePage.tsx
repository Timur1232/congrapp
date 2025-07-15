import React from 'react';
import { useNavigate } from 'react-router-dom';

const HomePage: React.FC = () => {
    const navigate = useNavigate();

    return (
        <div className="app-container">
            <h1 className="app-title">Congrapp</h1>
            <p className="app-subtitle">Приложение для напоминания о днях рождения</p>

            <div className="button-group">
                <button
                    className="primary-button"
                    onClick={() => navigate('/login')}
                >
                    Вход
                </button>
                <button
                    className="secondary-button"
                    onClick={() => navigate('/register')}
                >
                    Регистрация
                </button>
            </div>
        </div>
    );
};

export default HomePage;