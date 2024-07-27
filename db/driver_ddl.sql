-- Table: roles
CREATE TABLE roles (
    role_id INT AUTO_INCREMENT PRIMARY KEY,
    role_name VARCHAR(50) NOT NULL
);

-- Sample data for roles
INSERT INTO roles (role_name) VALUES
('admin'),
('driver'),
('client');

-- Table: users
CREATE TABLE users (
    user_id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(255) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Sample data for users
INSERT INTO users (username, password_hash, email) VALUES
('admin', 'hashed_password1', 'admin@example.com'),
('driver1', 'hashed_password2', 'driver1@example.com'),
('client1', 'hashed_password3', 'client1@example.com');

CREATE TABLE user_roles (
	user_role_id INT AUTO_INCREMENT PRIMARY KEY,
	user_id INT NOT NULL,
    role_id INT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
	FOREIGN KEY (user_id) REFERENCES users(user_id),
    FOREIGN KEY (role_id) REFERENCES roles(role_id)
);

INSERT INTO user_roles (user_id, role_id) VALUES (1,1), (2,2), (3,3);

-- Table: user_profiles
CREATE TABLE user_profiles (
    profile_id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    full_name VARCHAR(255) NOT NULL,
    address VARCHAR(255),
    phone_number VARCHAR(20),
    dob DATETIME NULL,
    license_number VARCHAR(255) NULL,
    license_issue_date DATETIME NULL,
    license_expiry_date DATETIME NULL,
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE
);

-- Sample data for user_profiles
INSERT INTO user_profiles (user_id, full_name, address, phone_number) VALUES
(1, 'Admin User', '123 Admin St.', '123-456-7890'),
(2, 'Driver One', '456 Driver Ave.', '234-567-8901'),
(3, 'Client One', '789 Client Rd.', '345-678-9012');

-- Sample data for about_us
CREATE TABLE content_type (
	content_type_id INT AUTO_INCREMENT PRIMARY KEY,
    type_name VARCHAR(255) NOT NULL
);

INSERT INTO content_type (type_name) VALUES
('Company Overview'), ('Service Description'), ('Process Description'), ('Training Material Information'), ('Q&A'), ('Post');
    
-- Table: content
CREATE TABLE content (
    content_id INT AUTO_INCREMENT PRIMARY KEY,
    content_type_id INT NOT NULL,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    is_public BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (content_type_id) REFERENCES content_type(content_type_id) ON DELETE CASCADE
);

-- Sample data for content
INSERT INTO content (title, content_type_id, description, is_public) VALUES
('About Us', 1, 'General info about the company', TRUE),
('Service Information', 2, 'Service A is useful to....', TRUE),
('Service Information', 3, 'Service B is useful to....', TRUE),
('Process Information', 2, 'Pricess A has the folllowing steps....', TRUE),
('Secure Training Material', 4, 'Training on how to anwer a call from a new customer...', TRUE),
('Secure Training Material', 4, 'Training on how to accept payment...', FALSE);

-- training material
CREATE TABLE training_material_type (
    material_type_id INT AUTO_INCREMENT PRIMARY KEY,
    material_type_name VARCHAR(255) NOT NULL UNIQUE
);

INSERT INTO training_material_type (material_type_name) VALUES ('Content'), ('Video'), ('Q&A'), ('MCQ');

CREATE TABLE training_material_category (
    material_category_id INT AUTO_INCREMENT PRIMARY KEY,
    category_name VARCHAR(255) NOT NULL UNIQUE
);

INSERT INTO training_material_category(category_name) VALUES ('Safety'), ('Legal'), ('Customer'), ('Payment');

-- Table: training_material
CREATE TABLE training_material (
    material_id INT AUTO_INCREMENT PRIMARY KEY,
    material_type_id INT NOT NULL,
    material_category_id INT NULL,
    is_public BOOLEAN DEFAULT FALSE,
    FOREIGN KEY (material_type_id) REFERENCES training_material_type(material_type_id) ON DELETE CASCADE,
    FOREIGN KEY (material_category_id) REFERENCES training_material_category(material_category_id) ON DELETE CASCADE
);

-- Sample data for training_material
INSERT INTO training_material (material_type_id, material_category_id) VALUES
(1,1),
(1,2),
(2,1),
(2,3),
(3, 3),
(3,2),
(4, 1),
(4,3);

-- Table: videos
CREATE TABLE videos (
    video_id INT AUTO_INCREMENT PRIMARY KEY,
    material_id INT NOT NULL,
    video_url VARCHAR(255) NOT NULL,
    FOREIGN KEY (material_id) REFERENCES training_material(material_id) ON DELETE CASCADE
);

-- Sample data for videos
INSERT INTO videos (material_id, video_url) VALUES
(3, 'https://vimeo.com/samplevideo'),
(4, 'https://vimeo.com/samplevideo2');

-- Table: questions_and_answers
CREATE TABLE questions_and_answers (
    qa_id INT AUTO_INCREMENT PRIMARY KEY,
    material_id INT NOT NULL,
    question TEXT NOT NULL,
    answer TEXT,
    is_public BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Sample data for questions_and_answers
INSERT INTO questions_and_answers (material_id, question, answer) VALUES
(5, 'What is the training duration?', 'The training duration is 30 days.'),
(6, 'What is the training difficulty level?', 'moderate to difficult.');

-- Table: exam_results
CREATE TABLE exam_result_types (
    result_type_id INT AUTO_INCREMENT PRIMARY KEY,
    result_type_name VARCHAR(50) NOT NULL
);

-- Sample data for exam_results
INSERT INTO exam_result_types (result_type_name) VALUES
('pass'),
('fail');

-- Table: exams
CREATE TABLE exams (
    exam_id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    date_started DATE,
    date_completed DATE,
    score INT,
    result_type_id INT NULL,
    certificate_url VARCHAR(255) NULL,
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE,
    FOREIGN KEY (result_type_id) REFERENCES exam_result_types(result_type_id)
);

-- Sample data for exams
INSERT INTO exams (user_id, date_started, date_completed, score, result_type_id, certificate_url) VALUES
(2, '2024-01-15', '2024-01-15', 85, 1, 'https://example.com/certificates/driver1_cert.pdf');

-- Table: exam_questions
CREATE TABLE user_exam_questions (
    exam_question_id INT AUTO_INCREMENT PRIMARY KEY,
    exam_id INT NOT NULL,
    qa_id INT NOT NULL, -- from material table
    user_answer TEXT,
    FOREIGN KEY (exam_id) REFERENCES exams(exam_id) ON DELETE CASCADE,
    FOREIGN KEY (qa_id) REFERENCES questions_and_answers(qa_id) ON DELETE CASCADE
);

-- Sample data for exam_questions
INSERT INTO user_exam_questions (exam_id, qa_id, user_answer) VALUES
(1, 1, '50 km/h');

-- user questions to admin
CREATE TABLE user_asked_questions (
	user_question_id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
	asked_question TEXT NOT NULL,
    admin_user_id INT NULL,
    admin_answer TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE,
    FOREIGN KEY (admin_user_id) REFERENCES users(user_id) ON DELETE CASCADE
);

INSERT INTO user_asked_questions (user_id, asked_question, admin_answer, admin_user_id) VALUES
(2, 'how to get a license?', null, null),
(3, 'how much do i get paid per kilometer?', 'depends', 1);
