from setuptools import setup

setup(
   name='test-print',
   version='1.0',
   description='test-print',
   long_description_content_type='text/markdown',
   long_description='test-print function',
   license='MIT',
   author='Stepanov Yaroslav',
   author_email='bigrashinboss1@gmail.com',
   url='https://github.com/zhbr112/fossdev',
   packages=['test-print'], 
   install_requires=[], # it is empty since we use standard python library
   extras_require={
        'test': [
            'pytest',
            'coverage',
        ],
   },
   python_requires='>=3',
)
