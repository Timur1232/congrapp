import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import authApi from '../AuthApi';
import './BirthdayEditPage.css';
import { BirthdayInfo, NotificationRecord } from "../AuthApi";

const BirthdayEditPage: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const { user } = useAuth();
    const [ isLoading, setIsLoading ] = useState<boolean>(true);

    const [birthday, setBirthday] = useState<BirthdayInfo | null>(null);

    const [personName, setPersonName] = useState('');
    const [birthdayDate, setBirthdayDate] = useState('');
    const [imageFile, setImageFile] = useState<File | null>(null);
    const [note, setNote] = useState<string | null>(null);

    const [notifications, setNotifications] = useState<NotificationRecord[]>([]);
    const [daysBefore, setDaysBefore] = useState(1);

    useEffect(() => {
        const fetchData = async () => {
            try {
                setIsLoading(true);
                
                const birthdayResponse = await authApi.fetchAuth(`/api/items?birthdayId=${id}`);
                const birthdayData = await birthdayResponse.json();
                setBirthday(birthdayData);
                setPersonName(birthdayData.personName);
                setBirthdayDate(birthdayData.birthdayDate);

                const notificationsResponse = await authApi.fetchAuth(`/api/notifications/all?birthdayId=${id}`);
                const notificationsData = await notificationsResponse.json();
                setNotifications(notificationsData);

            } catch (err) {
                console.error('Ошибка загрузки данных: ' + err);
            } finally {
                setIsLoading(false);
            }
        };

        fetchData();
    }, [id]);

    const handleCancel = async () => {
        navigate('/dashboard');
    }
    
    const handleSave = async () => {
        try {
            setIsLoading(true);
            
            const res = await authApi.fetchAuthJson<BirthdayInfo>(`/api/items?birthdayId=${id}`, {
                method: 'PUT',
                body: JSON.stringify({
                    personName,
                    birthdayDate,
                    note
                }),
                headers: { 'Content-Type': 'application/json' }
            });

            if (imageFile) {
                const formData = new FormData();
                formData.append('file', imageFile);
                
                await authApi.fetchAuth(`/api/images?birthdayId=${id}`, {
                    method: 'POST',
                    body: formData
                });
            }

            navigate('/dashboard');

        } catch (err) {
            console.error('Ошибка сохранения: ' + err);
        } finally {
            setIsLoading(false);
        }
    };

    const handleDelete = async () => {
        if (window.confirm('Удалить эту запись?')) {
            try {
                setIsLoading(true);
                await authApi.fetchAuth(`/api/items?birthdayId=${id}`, {
                    method: 'DELETE'
                })
                navigate('/dashboard');
            } catch (err) {
                console.error('Ошибка удаления: ' + err);
            } finally {
                setIsLoading(false);
            }
        }
    };

    const handleAddNotification = async () => {
        try {
            setIsLoading(true);
            
            const newNotification = await authApi.fetchAuthJson<NotificationRecord>(`/api/notifications?birthdayId=${id}`, {
                method: 'POST',
                body: JSON.stringify({daysBefore}),
                headers: { 'Content-Type': 'application/json' }
            });

            setNotifications([...notifications, newNotification]);
            
            setDaysBefore(1);

        } catch (err) {
            console.error('Ошибка добавления напоминания: ' + err);
        } finally {
            setIsLoading(false);
        }
    };

    const handleDeleteNotification = async (notificationId: number) => {
        try {
            setIsLoading(true);

            const deletedNotification = await authApi.fetchAuthJson<NotificationRecord>(`/api/notifications?birthdayId=${id}&notificationId=${notificationId}`, {
                method: 'DELETE'
            });

            setNotifications(notifications.filter(n => n.id !== deletedNotification.id));

        } catch (err) {
            console.error('Ошибка удаления напоминания: ' + err);
        } finally {
            setIsLoading(false);
        }
    };

    if (!birthday) {
        return <div className="error-message">Запись не найдена</div>;
    }

    return (
        <div className="birthday-edit-container">
            <h2>Редактирование дня рождения</h2>

            <div className="form-section">
                <div className="form-group">
                    <label>Имя</label>
                    <input
                        type="text"
                        value={personName}
                        onChange={(e) => setPersonName(e.target.value)}
                        disabled={isLoading}
                    />
                </div>

                <div className="form-group">
                    <label>Дата рождения</label>
                    <input
                        type="date"
                        value={birthdayDate}
                        onChange={(e) => setBirthdayDate(e.target.value)}
                        disabled={isLoading}
                        max={new Date().toISOString().split('T')[0]}
                    />
                </div>

                <div className="form-group">
                    <label>Изображение (JPG/JPEG)</label>
                    <input
                        type="file"
                        accept=".jpg,.jpeg"
                        onChange={(e) => setImageFile(e.target.files?.[0] || null)}
                        disabled={isLoading}
                    />
                </div>

                <div className="form-actions">
                    <button className="cancel-button"
                            onClick={handleCancel}
                            disabled={isLoading}
                    >
                        Отмена
                    </button>
                        
                    <button
                        className="confirm-button"
                        onClick={handleSave}
                        disabled={isLoading}
                    >
                        {isLoading ? 'Сохранение...' : 'Сохранить изменения'}
                    </button>
                    <button
                        className="delete-button"
                        onClick={handleDelete}
                        disabled={isLoading}
                    >
                        Удалить запись
                    </button>
                </div>
            </div>

            <div className="notifications-section">
                <h3>Напоминания</h3>

                {user?.emailVerified ? (
                    <>
                        {
                            <div className="notification-add-form">
                                <select
                                    value={daysBefore}
                                    onChange={(e) => setDaysBefore(parseInt(e.target.value))}
                                    disabled={isLoading}
                                >
                                    {[...Array(10)].map((_, i) => (
                                        <option key={i} value={i + 1}>
                                            {i + 1} {i === 0 ? 'день' : i < 4 ? 'дня' : 'дней'}
                                        </option>
                                    ))}
                                </select>

                                <button
                                    onClick={handleAddNotification}
                                    disabled={isLoading}
                                >
                                    Добавить напоминание
                                </button>
                            </div>
                        }

                        <div className="notifications-list">
                            {notifications.map(notification => (
                                <div key={notification.id} className="notification-item">
                  <span>
                    {notification.daysBefore} {notification.daysBefore === 1 ? 'день' : notification.daysBefore < 5 ? 'дня' : 'дней'} до
                  </span>

                                    <div className="notification-actions">
                                        <button
                                            onClick={() => handleDeleteNotification(notification.id)}
                                            disabled={isLoading}
                                            className="delete"
                                        >
                                            Удалить
                                        </button>
                                    </div>
                                </div>
                            ))}
                        </div>
                    </>
                ) : (
                    <div className="email-warning">
                        Для добавления напоминаний подтвердите ваш email!
                    </div>
                )}
            </div>
        </div>
    );
};

export default BirthdayEditPage;