import React, { useState, useEffect } from 'react';
import authApi from './AuthApi';

interface IProtectedImage {
    birthdayId: number;
    alt: string;
    className?: string;
}

const ImageLoader: React.FC<IProtectedImage> = ({ birthdayId, alt, className }) => {
    const [imageUrl, setImageUrl] = useState<string | null>(null);

    useEffect(() => {
        const fetchImage = async () => {
            try {
                const res= await authApi.fetchAuth(`/api/images?birthdayId=${birthdayId}`, {
                    method: 'GET',
                });

                if (!res.ok) {
                    console.error('Ошибка загрузки изображения');
                }

                const blob = await res.blob();
                const objectUrl = URL.createObjectURL(blob);
                setImageUrl(objectUrl);
            } catch (error) {
                console.error('Ошибка загрузки изображения', error);
            }
        };

        fetchImage();

        return () => {
            if (imageUrl) {
                URL.revokeObjectURL(imageUrl);
            }
        };
    }, [birthdayId]);

    if (!imageUrl) {
        return <div className="no-image">👤</div>;
    }

    return <img src={imageUrl} alt={alt} className={className} />;
};

export default ImageLoader;