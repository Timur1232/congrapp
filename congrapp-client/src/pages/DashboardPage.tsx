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

    const sortBirthdays = (items: BirthdayInfo[]): BirthdayInfo[] => {
        items.sort((a, b) => {
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
            
            const upcomingA = upcomingBirthday(a.birthdayDate);
            const upcomingB = upcomingBirthday(b.birthdayDate);
            
            return upcomingB.getTime() - upcomingA.getTime();
        });

        return items;
    };

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

                <div className="birthdays-list">
                    {sortBirthdays(birthdays).map((item) => (
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
                                    {new Date(item.birthdayDate).toLocaleDateString('ru-RU', {
                                        day: '2-digit',
                                        month: '2-digit'
                                    })}
                                </p>
                            </div>
                        </div>
                    ))}
                </div>
            </main>
        </div>
    );
};

export default DashboardPage;