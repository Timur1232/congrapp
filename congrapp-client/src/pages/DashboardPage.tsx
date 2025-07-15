import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import authApi from '../AuthApi';
import { BirthdayInfo } from "../AuthApi";
import './DashboardPage.css';
import ImageLoader from '../ImageLoader';

const DashboardPage: React.FC = () => {
    const navigate = useNavigate();
    const { user, logout } = useAuth();
    const [birthdays, setBirthdays] = useState<BirthdayInfo[]>([]);
    const [imageUrl, setImageUrl] = useState<string | null>(null); 

    useEffect(() => {
        const fetchBirthdays = async () => {
            try {
                const data: BirthdayInfo[] = await authApi.fetchAuthJson<BirthdayInfo[]>('/api/items/all', {
                    method: 'GET'
                });
                setBirthdays(data);
            } catch (err) {
                console.error('Ошибка загрузки данных: ' + err);
            } finally {
            }
        };

        fetchBirthdays();
    }, []);

    const handleLogout = () => {
        logout();
        navigate('/');
    };
    
    const upcomingBirthday = (dateStr: string): Date => {
        const today = new Date();
        const [year, month, day] = dateStr
            .split('-')
            .map(Number);
        const date = new Date(today.getFullYear(), month - 1, day);

        if (date < today) {
            date.setFullYear(today.getFullYear() + 1);
        }

        return date;
    };
    
    const isToday = (date: Date): boolean => {
        const today = new Date();
        return date.getDate() === today.getDate()
            && date.getMonth() == today.getMonth();
    } 
    
    const sortBirthdays = (items: BirthdayInfo[]): BirthdayInfo[] => {
        return [...items].sort((a, b) => {
            const upcomingA = upcomingBirthday(a.birthdayDate);
            const upcomingB = upcomingBirthday(b.birthdayDate);
            return upcomingB.getTime() - upcomingA.getTime();
        });
    };
    
    const sortedBirthdays = sortBirthdays(birthdays);

    const todayBirthdays = sortedBirthdays.filter(item => {
        const date = upcomingBirthday(item.birthdayDate);
        return isToday(date);
    });

    const upcomingBirthdays = sortedBirthdays.filter(item => {
        const date = upcomingBirthday(item.birthdayDate);
        return !isToday(date);
    });

    const handleAddBirthday = () => {
        navigate('/add-birthday');
    };

    const handleViewDetails = (id: number) => {
        navigate(`/birthday/${id}`);
    };

    return (
        <div className="dashboard-container">
            <header className="dashboard-header">
                <h1>Congrapp</h1>
                <div className="user-info">
                    <span>{user?.email}</span>
                    <button onClick={handleLogout}>Выйти</button>
                </div>
            </header>

            <main className="dashboard-content">
                <div className="toolbar">
                    <h2>Дни рождения</h2>
                    <button
                        className="add-button"
                        onClick={handleAddBirthday}
                    >
                        + Добавить
                    </button>
                </div>

                {todayBirthdays.length > 0 && (
                    <div className="birthday-section">
                        <h3>Сегодня празднуют</h3>
                        <div className="birthdays-list">
                            {todayBirthdays.map((item) => (
                                <div
                                    key={item.id}
                                    className="birthday-card today-card"
                                    onClick={() => handleViewDetails(item.id)}
                                >
                                    <div className="image-container">
                                        {item.hasImage ? (
                                            <ImageLoader
                                                birthdayId={item.id}
                                                alt={item.personName}
                                                className="birthday-image"
                                            />
                                        ) : (
                                            <div className="no-image">🎂</div>
                                        )}
                                    </div>

                                    <div className="info-container">
                                        <h3>{item.personName}</h3>
                                        <p>Сегодня!</p>
                                    </div>
                                </div>
                            ))}
                        </div>
                    </div>
                )}

                <div className="birthday-section">
                    <h3>Ближайшие дни рождения</h3>
                    <div className="birthdays-list">
                        {upcomingBirthdays.map((item) => (
                            <div
                                key={item.id}
                                className="birthday-card"
                                onClick={() => handleViewDetails(item.id)}
                            >
                                <div className="image-container">
                                    {item.hasImage ? (
                                        <ImageLoader
                                            birthdayId={item.id}
                                            alt={item.personName}
                                            className="birthday-image"
                                        />
                                    ) : (
                                        <div className="no-image">👤</div>
                                    )}
                                </div>

                                <div className="info-container">
                                    <h3>{item.personName}</h3>
                                    <p>
                                        {new Date(upcomingBirthday(item.birthdayDate)).toLocaleDateString('ru-RU', {
                                            day: '2-digit',
                                            month: 'long'
                                        })}
                                    </p>
                                </div>
                            </div>
                        ))}
                    </div>
                </div>
            </main>
        </div>
    );
};

export default DashboardPage;