import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import authApi from '../AuthApi';
import './AddBirthdayPage.css';

const AddBirthdayPage: React.FC = () => {
    const navigate = useNavigate();

    const [personName, setPersonName] = useState('');
    const [birthdayDate, setBirthdayDate] = useState('');
    const [imageFile, setImageFile] = useState<File | null>(null);
    const [isSubmitting, setIsSubmitting] = useState(false);

    const handleNameChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setPersonName(e.target.value);
    };

    const handleDateInput  = (e: React.ChangeEvent<HTMLInputElement>) => {
        setBirthdayDate(e.target.value);
    };

    const handleFileInput = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (e.target.files && e.target.files[0]) {
            const file = e.target.files[0];
            setImageFile(file);
        }
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setIsSubmitting(true);

        try {
            const birthdayData = { 
                personName, 
                birthdayDate: new Date(birthdayDate).toISOString().split('T')[0]
            };
            
            const birthdayResponse = await authApi.fetchAuth('/api/items', {
                method: 'POST',
                body: JSON.stringify(birthdayData),
                headers: { 'Content-Type': 'application/json' }
            });
            
            const newBirthday = await birthdayResponse.json();
            
            if (imageFile) {
                const formData = new FormData();
                formData.append('file', imageFile);
              
                await authApi.fetchAuth(`/api/images?birthdayId=${newBirthday.id}`, {
                    method: 'POST',
                    body: formData
                });
            }
            
            navigate('/dashboard');
        } catch (err) {
            console.error('Ошибка при создании записи: ' + err);
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <div className="add-birthday-container">
            <h2>Добавить день рождения</h2>

            <form onSubmit={handleSubmit} className="birthday-form">
                <div className="form-group">
                    <label htmlFor="personName">Имя *</label>
                    <input
                        id="personName"
                        type="text"
                        value={personName}
                        onChange={handleNameChange}
                        required
                        disabled={isSubmitting}
                    />
                </div>

                <div className="form-group">
                    <label htmlFor="birthdayDate">Дата рождения *</label>
                    <input
                        id="birthdayDate"
                        type="date"
                        value={birthdayDate}
                        onChange={handleDateInput}
                        required
                        disabled={isSubmitting}
                        max={new Date().toISOString().split('T')[0]}
                    />
                </div>

                <div className="form-group">
                    <label htmlFor="imageUpload">Изображение (JPG/JPEG)</label>
                    <input
                        id="imageUpload"
                        type="file"
                        accept=".jpg,.jpeg"
                        onChange={handleFileInput}
                        disabled={isSubmitting}
                    />
                </div>

                <div className="form-actions">
                    <button
                        type="button"
                        className="cancel-button"
                        onClick={() => navigate('/dashboard')}
                        disabled={isSubmitting}
                    >
                        Отмена
                    </button>
                    <button
                        type="submit"
                        className="submit-button"
                        disabled={isSubmitting}
                    >
                        {isSubmitting ? 'Сохранение...' : 'Сохранить'}
                    </button>
                </div>
            </form>
        </div>
    );
};

export default AddBirthdayPage; 
